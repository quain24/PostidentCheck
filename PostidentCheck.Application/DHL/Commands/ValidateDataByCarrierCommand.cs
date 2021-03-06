using MediatR;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Extensions;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using Postident.Core.Enums;
using SharedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.DHL.Commands
{
    public class ValidateDataByCarrierCommand : IRequest<CommandResult<int>>
    {
        public Carrier[] Carriers { get; }

        public ValidateDataByCarrierCommand(params Carrier[] carriers)
        {
            Carriers = carriers ?? throw new ArgumentNullException(nameof(carriers), "This command requires one or more 'carriers' to be executed");
            if (Carriers.IsNullOrEmpty())
                throw new ArgumentOutOfRangeException(nameof(carriers), "There are no 'carriers' in passed in collection.");
        }
    }

    public class ValidateDataByCarrierCommandHandler : IRequestHandler<ValidateDataByCarrierCommand, CommandResult<int>>
    {
        private const string Name = "Validation Command handler - specified carriers";
        private readonly IDataPackReadRepository _readRepository;
        private readonly IInfoPackDbContext _writeContext;
        private readonly IValidationService _validationService;
        private readonly IReadModelToDataPackMapper _mapper;
        private readonly ILogger<ValidateDataByCarrierCommandHandler> _logger;

        public ValidateDataByCarrierCommandHandler(IDataPackReadRepository repository,
            IInfoPackDbContext writeContext,
            IValidationService validationService,
            IReadModelToDataPackMapper mapper,
            ILogger<ValidateDataByCarrierCommandHandler> logger)
        {
            _readRepository = repository;
            _writeContext = writeContext;
            _validationService = validationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CommandResult<int>> Handle(ValidateDataByCarrierCommand request, CancellationToken cancellationToken)
        {
            using var scope = _logger?.BeginScope(Name);
            var dataToValidate = await RetrieveReadModels(request.Carriers, cancellationToken).ConfigureAwait(false);

            if (dataToValidate is null)
                return DatabaseErrorOccurred();
            if (dataToValidate.Count == 0)
                return NoDataPacksToCheck();

            var mappedData = _mapper.Map(dataToValidate);
            var results = (await _validationService.Validate(mappedData, cancellationToken)).ToList();

            _logger?.LogWriteModel(results);
            return await UpdateDatabase(results);
        }

        private async Task<List<DataPackReadModel>> RetrieveReadModels(IEnumerable<Carrier> carriers, CancellationToken token)
        {
            try
            {
                _logger?.LogInformation("{0}: Trying to validate information from db...", Name);
                return await _readRepository.GetDataPacks(carriers.ToArray(), token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{0}: Could not get data from database, exception has occurred - ending command", Name);
                return null;
            }
        }

        private static CommandResult<int> DatabaseErrorOccurred() => CommandResult<int>.InvalidResult(-1, "Database error");

        private CommandResult<int> NoDataPacksToCheck()
        {
            _logger?.LogInformation("{0}: Database query returned no data to be check", Name);
            return CommandResult<int>.ValidResult(0, "Database query returned no data to be checked");
        }

        private async Task<CommandResult<int>> UpdateDatabase(ICollection<InfoPackWriteModel> data)
        {
            try
            {
                var updatedDataPacksAmount = await _writeContext.SaveChangesAsync(data).ConfigureAwait(false);

                if (updatedDataPacksAmount > 0)
                    _logger?.LogInformation("{0}: Database updated successfully, updated {1}/{2} element(s).", Name, updatedDataPacksAmount, data.Count);
                else
                    _logger?.LogInformation("{0}: No informations were updated in database.", Name);
                return CommandResult<int>.ValidResult(updatedDataPacksAmount, $"Database updated successfully, updated {updatedDataPacksAmount} element(s).");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{0}: Updating database failed - exception was thrown", Name);
                return CommandResult<int>.InvalidResult(-1, "Updating database failed - exception was thrown");
            }
        }
    }
}
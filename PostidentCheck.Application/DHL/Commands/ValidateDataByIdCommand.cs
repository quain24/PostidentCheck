using MediatR;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Extensions;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using SharedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.DHL.Commands
{
    public class ValidateDataByIdCommand : IRequest<CommandResult<int>>
    {
        public string[] Ids { get; }

        public ValidateDataByIdCommand(params string[] ids)
        {
            Ids = ids ?? throw new ArgumentNullException(nameof(ids), "This command requires one or more ID to be executed");
            if (Ids.IsNullOrEmpty())
                throw new ArgumentOutOfRangeException(nameof(ids), "There are no ID's in passed in collection.");
        }
    }

    public class ValidateDataByIdCommandHandler : IRequestHandler<ValidateDataByIdCommand, CommandResult<int>>
    {
        private const string Name = "Validation Command handler - specified ID's";
        private readonly IDataPackReadRepository _readRepository;
        private readonly IInfoPackDbContext _writeContext;
        private readonly IValidationService _validationService;
        private readonly IReadModelToDataPackMapper _mapper;
        private readonly ILogger<ValidateDataByIdCommandHandler> _logger;

        public ValidateDataByIdCommandHandler(IDataPackReadRepository repository,
            IInfoPackDbContext writeContext,
            IValidationService validationService,
            IReadModelToDataPackMapper mapper,
            ILogger<ValidateDataByIdCommandHandler> logger)
        {
            _readRepository = repository;
            _writeContext = writeContext;
            _validationService = validationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CommandResult<int>> Handle(ValidateDataByIdCommand request, CancellationToken cancellationToken)
        {
            using var scope = _logger?.BeginScope(Name);
            var dataToValidate = await RetrieveReadModels(SanitizeInput(request.Ids), cancellationToken).ConfigureAwait(false);

            if (dataToValidate is null)
                return DatabaseErrorOccurred();
            if (dataToValidate.Count == 0)
                return NoDataPacksToCheck();

            var mappedData = _mapper.Map(dataToValidate);
            var results = (await _validationService.Validate(mappedData, cancellationToken)).ToList();

            _logger?.LogWriteModel(results);
            return await UpdateDatabase(results);
        }

        private static IEnumerable<string> SanitizeInput(IEnumerable<string> ids)
        {
            var tmp = new HashSet<string>();
            foreach (var id in ids)
            {
                if (string.IsNullOrWhiteSpace(id) is false)
                    tmp.Add(id);
            }

            return tmp;
        }

        private async Task<List<DataPackReadModel>> RetrieveReadModels(IEnumerable<string> ids, CancellationToken token)
        {
            try
            {
                _logger?.LogInformation("{0}: Trying to validate information from db...", Name);
                return await _readRepository.GetDataPacks(ids.ToArray(), token).ConfigureAwait(false);
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
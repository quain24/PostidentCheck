using MediatR;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.DHL.Commands
{
    public class ValidateAllDataCommand : IRequest<bool>
    {
    }

    public class ValidateAllDataCommandHandler : IRequestHandler<ValidateAllDataCommand, bool>
    {
        private const string Name = "Validation Command handler";
        private readonly IDataPackReadRepository _readRepository;
        private readonly IInfoPackDbContext _writeContext;
        private readonly IValidationService _validationService;
        private readonly IReadModelToDataPackMapper _mapper;
        private readonly ILogger<ValidateAllDataCommandHandler> _logger;

        public ValidateAllDataCommandHandler(IDataPackReadRepository repository,
            IInfoPackDbContext writeContext,
            IValidationService validationService,
            IReadModelToDataPackMapper mapper,
            ILogger<ValidateAllDataCommandHandler> logger)
        {
            _readRepository = repository;
            _writeContext = writeContext;
            _validationService = validationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> Handle(ValidateAllDataCommand request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("{0}: Trying to validate all possible information from db...", Name);
            var dataToValidate = await RetrieveReadModels(cancellationToken).ConfigureAwait(false);

            if (dataToValidate is null)
                return DatabaseErrorOccurred();
            if (dataToValidate.Count == 0)
                return NoDataPacksToCheck();

            var mappedData = _mapper.Map(dataToValidate);

            var results = await _validationService.Validate(mappedData, cancellationToken);

            results.ToList().OrderBy(r => r.Id).ToList().ForEach(r =>
            {
                Console.WriteLine("======================================================");
                Console.WriteLine("Check status: " + r.CheckStatus);
                Console.WriteLine("ID: " + r.Id);
                Console.WriteLine("Message: " + r.Message);
                Console.WriteLine("======================================================");
            });

            return await UpdateDatabase(results);
        }

        private async Task<List<DataPackReadModel>> RetrieveReadModels(CancellationToken token)
        {
            try
            {
                return await _readRepository.GetDataPacks(token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{0}: Could not get data from database, exception has occurred - ending command", Name);
                return new List<DataPackReadModel>();
            }
        }

        private static bool DatabaseErrorOccurred() => false; // method added for readability

        private bool NoDataPacksToCheck()
        {
            _logger?.LogInformation("{0}: Database query returned no parcels to be check - ending command", Name);
            return true;
        }

        private async Task<bool> UpdateDatabase(IEnumerable<InfoPackWriteModel> data)
        {
            try
            {
                var updatedParcelsAmount = await _writeContext.SaveChangesAsync(data);

                if (updatedParcelsAmount > 0)
                    _logger?.LogInformation("{0}: Database updated successfully, updated {1} element(s).", Name, updatedParcelsAmount);
                else
                    _logger?.LogInformation("{0}: No informations were updated in database.", Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "{0}: Updating database failed - exception was thrown", Name);
                return false;
            }
        }
    }
}
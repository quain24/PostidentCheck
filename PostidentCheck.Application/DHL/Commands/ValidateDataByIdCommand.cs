using MediatR;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
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
    public class ValidateDataByIdCommand : IRequest<bool>
    {
        public IEnumerable<string> Ids { get; }

        public ValidateDataByIdCommand(IEnumerable<string> ids)
        {
            Ids = ids ?? throw new ArgumentNullException(nameof(ids), "This command requires one or more ID to be executed");
            if (Ids.IsNullOrEmpty())
                throw new ArgumentOutOfRangeException(nameof(ids), "There are no ID's in passed in collection.");
        }
    }

    public class ValidateDataByIdCommandHandler : IRequestHandler<ValidateDataByIdCommand, bool>
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

        public async Task<bool> Handle(ValidateDataByIdCommand request, CancellationToken cancellationToken)
        {
            _logger?.LogInformation("{0}: Trying to validate information from db...", Name);
            var dataToValidate = await RetrieveReadModels(request.Ids, cancellationToken).ConfigureAwait(false);

            if (dataToValidate is null)
                return DatabaseErrorOccurred();
            if (dataToValidate.Count == 0)
                return NoDataPacksToCheck();

            var mappedData = _mapper.Map(dataToValidate);

            mappedData.ToList().ForEach(m =>
            {
                Console.WriteLine("======================================================");
                Console.WriteLine("Id: " + m.Id);
                Console.WriteLine("Carrier: " + m.Carrier);
                Console.WriteLine("Check status: " + m.DataPackChecked);
                Console.WriteLine("City: " + m.Address.City);
                Console.WriteLine("Country: " + m.Address.CountryCode);
                Console.WriteLine("Postident: " + m.Address.PostIdent);
                Console.WriteLine("Street: " + m.Address.Street);
                Console.WriteLine("Street nr: " + m.Address.StreetNumber);
                Console.WriteLine("ZipCode: " + m.Address.ZipCode);
                Console.WriteLine("======================================================");
            });

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

        private async Task<List<DataPackReadModel>> RetrieveReadModels(IEnumerable<string> ids, CancellationToken token)
        {
            try
            {
                return await _readRepository.GetDataPacks(ids.ToArray(), token);
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
                var updatedParcelsAmount = await _writeContext.SaveChangesAsync(data).ConfigureAwait(false);

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
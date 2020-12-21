using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.DHL.Interfaces;
using Postident.Core.Entities;
using Postident.Core.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Application.DHL.Commands
{
    public class DhlUpdateParcelStatusesCommand : IRequest<bool>
    {
    }

    public class DhlUpdateParcelStatusesHandler : IRequestHandler<DhlUpdateParcelStatusesCommand, bool>
    {
        private readonly IDataPackReadRepository _repository;
        private readonly IDhlApiService _apiService;
        private readonly IValidator<DhlResponseDto> _dhlResponseValidator;
        private readonly ILogger<DhlUpdateParcelStatusesHandler> _logger;

        public DhlUpdateParcelStatusesHandler(IDataPackReadRepository repository,
            IDhlApiService apiService,
            IValidator<DhlResponseDto> dhlResponseValidator,
            ILogger<DhlUpdateParcelStatusesHandler> logger)
        {
            _repository = repository;
            _apiService = apiService;
            _dhlResponseValidator = dhlResponseValidator;
            _logger = logger;
        }

        public async Task<bool> Handle(DhlUpdateParcelStatusesCommand request, CancellationToken cancellationToken)
        {
            var parcelsToUpdate = await RetrieveParcelsFromDatabase(cancellationToken).ConfigureAwait(false);

            if (parcelsToUpdate is null)
                return DatabaseErrorOccurred();
            if (parcelsToUpdate.Count == 0)
                return NoParcelsToCheck();

            // var apiResponses = await _apiService
            //.GetParcelInfoFromProviderAsync(parcelsToUpdate, cancellationToken)
            // .ConfigureAwait(false);

            //UpdateParcels(apiResponses, parcelsToUpdate);
            return await UpdateDatabase();
        }

        private async Task<List<DataPackReadModel>> RetrieveParcelsFromDatabase(CancellationToken token)
        {
            try
            {
                return await Task.FromResult(new List<DataPackReadModel>()); //await _repository.GetParcelsSentBy(new[] { Carrier.DHL, Carrier.DHL_Sperr }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: Could not get data from database, exception has occurred - ending command", nameof(Carrier.DHL));
                return new List<DataPackReadModel>();
            }
        }

        private static bool DatabaseErrorOccurred() => false; // method added for readability

        private bool NoParcelsToCheck()
        {
            _logger.LogInformation("{0}: Database query returned no parcels to be check - ending command", nameof(Carrier.DHL));
            return true;
        }

        private async Task<bool> UpdateDatabase()
        {
            try
            {
                // todo actual save implementation in save repository
                var updatedParcelsAmount = 0;//await _repository.UpdateDataPacksTableAsync();
                if (updatedParcelsAmount > 0)
                    _logger.LogInformation("{0}: Database updated successfully, updated {1} parcel(s).", nameof(Carrier.DHL), updatedParcelsAmount);
                else
                    _logger.LogInformation("{0}: No parcels were updated in database.", nameof(Carrier.DHL));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0}: Updating database failed - exception was thrown", nameof(Carrier.DHL));
                return false;
            }
        }
    }
}
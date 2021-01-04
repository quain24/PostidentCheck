using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Application.DHL;
using Postident.Application.DHL.Interfaces;
using Postident.Core.Entities;
using Postident.Infrastructure.Interfaces;
using SharedExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Postident.Infrastructure.Services
{
    public class ValidationService : IValidationService
    {
        private const string Name = "Validation service";
        private readonly IOfflineDataPackValidationService<InfoPackWriteModel> _offlineValidationService;
        private readonly IDhlApiService _onlineValidationService;
        private readonly IDhlResponseToWriteModelMapper _mapper;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(
            IOfflineDataPackValidationService<InfoPackWriteModel> offlineValidationService,
            IDhlApiService onlineValidationService,
            IDhlResponseToWriteModelMapper mapper,
            ILogger<ValidationService> logger)
        {
            _offlineValidationService = offlineValidationService;
            _onlineValidationService = onlineValidationService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// <inheritdoc cref="IValidationService.Validate"/>
        /// </summary>
        /// <param name="dataPacks">Collection to be validated</param>
        /// <param name="token">Cancels running validation</param>
        /// <returns>A collection of <see cref="InfoPackWriteModel"/></returns>
        public async Task<IEnumerable<InfoPackWriteModel>> Validate(IEnumerable<DataPack> dataPacks, CancellationToken token)
        {
            if (dataPacks.IsNullOrEmpty())
            {
                _logger?.LogInformation("{0}: Nothing to validate, returning empty collection...", Name);
                return Array.Empty<InfoPackWriteModel>();
            }

            _logger?.LogInformation("{0}: Validating {1} Data Packs.", Name, dataPacks.Count());
            _logger?.LogInformation("{0}: Beginning offline validation...", Name);

            var offlineValidatedDataPacks = _offlineValidationService
                .FilterOutInvalidDataPacksFrom(dataPacks.ToList(), out var processedData)
                .ToList();

            var onlineValidatedDataPacks = await PerformOnlineValidation(offlineValidatedDataPacks, token).ConfigureAwait(false);

            return processedData.Concat(_mapper.Map(onlineValidatedDataPacks));
        }

        private async Task<List<DhlMainResponseDto>> PerformOnlineValidation(List<DataPack> dataPacks, CancellationToken token)
        {
            if (dataPacks.IsNullOrEmpty())
            {
                _logger?.LogInformation("{0}: Nothing to validate online...");
                return new List<DhlMainResponseDto>(0);
            }

            _logger?.LogInformation("{0}: Beginning online validation...", Name);
            var apiResponses = (await _onlineValidationService.GetResponsesFromApiAsync(dataPacks, token).ConfigureAwait(false)).ToList();

            var validApiResponses = new List<DhlMainResponseDto>();
            foreach (var response in apiResponses)
            {
                if (response.Responses.Count > 0)
                    validApiResponses.Add(response);
                else
                {
                    _logger?.LogWarning("{0}: One of responses contained no necessary information - logging info and purging: Error code: {1} || Message: {2}",
                        Name, response.MainFaultCode ?? "---", response.MainFaultText ?? "---");
                }
            }

            return validApiResponses;
        }
    }
}
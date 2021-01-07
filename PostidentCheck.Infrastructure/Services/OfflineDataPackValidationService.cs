using FluentValidation;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Extensions;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using Postident.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Postident.Infrastructure.Services
{
    public class OfflineDataPackValidationService : IOfflineDataPackValidationService<InfoPackWriteModel>
    {
        private const string Name = "DataPack offline validator";
        private readonly IInvalidValidationToWriteModelMapper<InfoPackWriteModel> _mapper;
        private readonly ILogger<OfflineDataPackValidationService> _logger;

        public OfflineDataPackValidationService(
            IValidator<DataPack> dataPackValidator,
            IInvalidValidationToWriteModelMapper<InfoPackWriteModel> mapper,
            ILogger<OfflineDataPackValidationService> logger)
        {
            _mapper = mapper;
            _logger = logger;
            DataPackValidator = dataPackValidator;
        }

        private IValidator<DataPack> DataPackValidator { get; }

        /// <summary>
        /// <inheritdoc cref="IOfflineDataPackValidationService{TInvalidResultsReturnType}.FilterOutInvalidDataPacksFrom"/>
        /// </summary>
        /// <param name="dataPacks">Collection of <see cref="DataPackReadModel"/> to be validated</param>
        /// <param name="invalidEntries">Collection of <see cref="InfoPackWriteModel"/> containing all of invalid <see cref="DataPackReadModel"/> translated to write models</param>
        /// <returns>Collection of valid <see cref="DataPackReadModel"/></returns>
        /// <exception cref="ArgumentNullException">When given data packs are null</exception>
        public IEnumerable<DataPack> FilterOutInvalidDataPacksFrom(IList<DataPack> dataPacks, out IEnumerable<InfoPackWriteModel> invalidEntries)
        {
            _ = dataPacks ?? throw new ArgumentNullException(nameof(dataPacks));
            invalidEntries = null;

            if (dataPacks.Count == 0)
            {
                _logger?.LogInformation("{0}: No data packs to filter.");
                invalidEntries = Array.Empty<InfoPackWriteModel>();
                return Array.Empty<DataPack>();
            }

            _logger?.LogInformation("{0}: Checking {1} data pack(s)...", Name, dataPacks.Count);

            var validEntries = new List<DataPack>();
            var tmpInvalidEntries = new List<InfoPackWriteModel>();
            dataPacks.ToList().ForEach(d =>
            {
                var result = DataPackValidator.Validate(d);
                if (result.IsValid)
                {
                    validEntries.Add(d);
                    return;
                }
                _logger?.LogError("{0}: ID {1} - {2}", Name, string.IsNullOrWhiteSpace(d.Id) ? "Unknown" : d.Id, result.CombinedErrors());
                if (result.Errors.Any(v => v.ErrorCode == "id_missing"))
                {
                    _logger?.LogError("{0}: Missing id - data pack will NOT be processed further even as invalid - purging!");
                    return;
                }

                tmpInvalidEntries.Add(_mapper.MapInvalidResult(result, d.Id));
            });

            _logger?.LogInformation("{0}: Check finished, returning {1} valid data packs and {2} invalid entries", Name, validEntries.Count, tmpInvalidEntries.Count);
            invalidEntries = tmpInvalidEntries;
            return validEntries;
        }
    }
}
using FluentValidation;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Extensions;
using Postident.Application.Common.Interfaces;
using Postident.Core.Entities;
using Postident.Infrastructure.Interfaces;
using SharedExtensions;
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
        private IValidator<DataPackReadModel> DataPackValidator { get; }

        public OfflineDataPackValidationService(
            IValidator<DataPackReadModel> dataPackValidator,
            IInvalidValidationToWriteModelMapper<InfoPackWriteModel> mapper,
            ILogger<OfflineDataPackValidationService> logger)
        {
            _mapper = mapper;
            _logger = logger;
            DataPackValidator = dataPackValidator;
        }

        /// <summary>
        /// <inheritdoc cref="IOfflineDataPackValidationService{TInvalidResultsReturnType}.FilterOutInvalidDataPacksFrom"/>
        /// </summary>
        /// <param name="dataPacks">Collection of <see cref="DataPackReadModel"/> to be validated</param>
        /// <param name="invalidEntries">Collection of <see cref="InfoPackWriteModel"/> containing all of invalid <see cref="DataPackReadModel"/> translated to write models</param>
        /// <returns>Collection of valid <see cref="DataPackReadModel"/></returns>
        public IEnumerable<DataPackReadModel> FilterOutInvalidDataPacksFrom(IList<DataPackReadModel> dataPacks, out IEnumerable<InfoPackWriteModel> invalidEntries)
        {
            invalidEntries = null;
            var tmpInvalidEntries = new List<InfoPackWriteModel>();

            var validEntries = new List<DataPackReadModel>();

            if (dataPacks.IsNullOrEmpty())
            {
                _logger?.LogInformation("{0}: No data packs to filter.");
                return Array.Empty<DataPackReadModel>();
            }

            _logger?.LogInformation("{0}: Checking {1} data pack(s)...", Name, dataPacks.Count);

            dataPacks.ToList().ForEach(d =>
            {
                var result = DataPackValidator.Validate(d);
                if (result.IsValid)
                {
                    validEntries.Add(d);
                    return;
                }

                tmpInvalidEntries.Add(_mapper.MapInvalidResult(result, d.Id));

                _logger?.LogWarning("{0}: ID {1} - {2}", Name, string.IsNullOrWhiteSpace(d.Id) ? "Unknown" : d.Id, result.CombinedErrors());
            });

            _logger?.LogInformation("{0}: Check finished, returning {1} valid data packs and {2} invalid entries", Name, validEntries.Count, invalidEntries.Count());
            invalidEntries = tmpInvalidEntries;
            return validEntries;
        }
    }
}
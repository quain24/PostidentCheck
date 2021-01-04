using Microsoft.Extensions.Logging;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using Postident.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Postident.Application.Common.Interfaces;

namespace Postident.Infrastructure.Mappers
{
    public class ReadModelToDataPackMapper : IReadModelToDataPackMapper
    {
        private readonly ILogger<ReadModelToDataPackMapper> _logger;
        private const string Name = "ReadModel to DataPack mapper";

        public ReadModelToDataPackMapper(ILogger<ReadModelToDataPackMapper> logger)
        {
            _logger = logger;
        }

        public ReadModelToDataPackMapper(DefaultNamingMap namingMap, ILogger<ReadModelToDataPackMapper> logger)
        {
            NamingMap = namingMap;
            _logger = logger;
        }

        private DefaultNamingMap NamingMap { get; }
        private readonly Regex _splitAddressRegex = new(@"^([\w\.\-\, ]+) ([0-9]{1,5})\s?([\w\.\-/]*)$");

        /// <summary>
        /// <inheritdoc cref="IReadModelToDataPackMapper.Map(DataPackReadModel)"/>
        /// </summary>
        /// <param name="dataPackReadModel">Entity from database</param>
        /// <returns><see cref="DataPack"/> created from provided <paramref name="dataPackReadModel"/></returns>
        /// <exception cref="ArgumentNullException">When trying to map <see langword="null"/></exception>
        public DataPack Map(DataPackReadModel dataPackReadModel)
        {
            _ = dataPackReadModel ?? throw new ArgumentNullException(nameof(dataPackReadModel));
            var (street, number) = SplitStreet(dataPackReadModel);

            return new DataPack()
            {
                Id = dataPackReadModel.Id.Trim(),
                Carrier = dataPackReadModel.Carrier,
                Address = new Address()
                {
                    City = dataPackReadModel.City.Trim(),
                    CountryCode = dataPackReadModel.CountryCode.Trim(),
                    PostIdent = dataPackReadModel.PostIdent.Trim(),
                    Street = street,
                    StreetNumber = number,
                    ZipCode = dataPackReadModel.ZipCode.Trim()
                },
                DataPackChecked = dataPackReadModel.DataPackChecked
            };
        }

        /// <summary>
        /// <inheritdoc cref="IReadModelToDataPackMapper.Map(IEnumerable{DataPackReadModel})"/>
        /// </summary>
        /// <param name="dataPackReadModels">Entities from database</param>
        /// <returns>Collection of <see cref="DataPack"/> created from provided <paramref name="dataPackReadModels"/></returns>
        /// <exception cref="ArgumentNullException">When trying to map <see langword="null"/></exception>
        public IEnumerable<DataPack> Map(IEnumerable<DataPackReadModel> dataPackReadModels) =>
            dataPackReadModels?.Select(this.Map) ?? throw new ArgumentNullException(nameof(dataPackReadModels));

        /// <summary>
        /// Tries to split provided street and street number from one string into separate ones
        /// </summary>
        /// <param name="dataPack"></param>
        /// <returns><see cref="ValueTuple"/> containing street name and street number</returns>
        private (string street, string number) SplitStreet(DataPackReadModel dataPack)
        {
            var result = _splitAddressRegex.Match(dataPack.Street.Trim());
            if (!result.Success || result.Groups.Count == 1)
            {
                _logger?.LogWarning("{0}: ID: {1} - could not split street into separate name and number - mapping as street without number.", Name, dataPack.Id);
                return (dataPack.Street.Trim(), string.Empty);
            }

            var streetMatch = NamingMap is not null && NamingMap.TryGetDefaultNameFor(result.Groups[1].Value.Trim(), out var mappedName) ?
                mappedName :
                result.Groups[1].Value.Trim();

            var numberMatch = string.Empty;

            // 0 is whole match, 1 is street name, so start from 2
            for (var i = 2; i < result.Groups.Count; i++)
            {
                numberMatch += result.Groups[i].Value.Trim();
            }

            return (streetMatch, numberMatch);
        }
    }
}
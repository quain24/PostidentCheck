using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using Postident.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Postident.Infrastructure.Mappers
{
    public class ReadModelToDataPackMapper : IReadModelToDataPackMapper
    {
        private readonly ILogger<IReadModelToDataPackMapper> _logger;
        private const string Name = "ReadModel to DataPack mapper";

        public ReadModelToDataPackMapper(ILogger<IReadModelToDataPackMapper> logger)
        {
            _logger = logger;
        }

        public ReadModelToDataPackMapper(DefaultNamingMap namingMap, ILogger<IReadModelToDataPackMapper> logger)
        {
            NamingMap = namingMap;
            _logger = logger;
        }

        private DefaultNamingMap NamingMap { get; }

        private readonly Dictionary<string, Regex> _regexMap = new(StringComparer.InvariantCultureIgnoreCase)
        {
            { "default", new Regex(@"^(?<street_name>[\S ]+?)\s*(?<number>\d+\s*[a-zA-Z]*\s*([-\/]\s*\d*\s*\w?\s*)*)$") },
            { "gb", new Regex(@"^(?<number>\d+) ?([A-Za-z](?= ))? (?<street_name>.*?) (?<street_suffix>[^ ]+?) ?((?<= )APT)? ?((?<= )\d*)?$") }
        };

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
                Id = dataPackReadModel.Id?.Trim() ?? string.Empty,
                Carrier = dataPackReadModel.Carrier,
                Address = new Address()
                {
                    City = dataPackReadModel.City?.Trim() ?? string.Empty,
                    CountryCode = dataPackReadModel.CountryCode?.Trim() ?? string.Empty,
                    PostIdent = dataPackReadModel.PostIdent?.Trim() ?? string.Empty,
                    Street = street,
                    StreetNumber = number,
                    ZipCode = dataPackReadModel.ZipCode?.Trim() ?? string.Empty
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
        public IEnumerable<DataPack> Map(IEnumerable<DataPackReadModel> dataPackReadModels)
        {
            var output = new List<DataPack>(dataPackReadModels.Count());
            foreach (var entry in dataPackReadModels)
            {
                output.Add(Map(entry));
            }
            return output;
        }

        /// <summary>
        /// Tries to split provided street and street number from one string into separate ones
        /// </summary>
        /// <param name="dataPack"></param>
        /// <returns><see cref="ValueTuple"/> containing street name and street number</returns>
        private (string street, string number) SplitStreet(DataPackReadModel dataPack)
        {
            if (string.IsNullOrWhiteSpace(dataPack.Street))
            {
                return (string.Empty, string.Empty);
            }

            var streetData = dataPack.Street.Trim();
            var result = FindMatchFor(streetData, dataPack.CountryCode);

            if (!result.Success || result.Groups.Count == 1)
            {
                _logger?.LogWarning("{0}: ID: {1} - could not split street into separate name and number" +
                    " - mapping as street without number.", Name, dataPack.Id);
                return (streetData, string.Empty);
            }

            var streetMatch = NamingMap is not null && NamingMap.TryGetDefaultNameFor(result.Groups["street_name"].Value.Trim(), out var mappedName) ?
                string.Join(' ', mappedName, result.Groups["street_suffix"].Value).Trim() :
                string.Join(' ', result.Groups["street_name"].Value.Trim(), result.Groups["street_suffix"].Value).Trim();

            var numberMatch = result.Groups["number"].Value;

            return (streetMatch, numberMatch);
        }

        /// <summary>
        /// If able, will return match for street name and number using regex for given <paramref name="countryCode"/>.<br/>
        /// If there is no such regex or match is not found, then it will try to match with default regex and return result.
        /// </summary>
        /// <param name="countryCode">2 - 3 letter iso country code</param>
        /// <param name="streetData">Name and number to split</param>
        /// <returns>Either specific match from country code regex or if none found / not matched then match using default regex</returns>
        private Match FindMatchFor(string streetData, string countryCode)
        {
            if (!string.IsNullOrWhiteSpace(countryCode) && _regexMap.ContainsKey(countryCode.Trim()))
            {
                _logger?.LogTrace("{0}: Found regex for \"{1}\" - trying to match...", Name, countryCode);
                var match = _regexMap[countryCode].Match(streetData);
                if (match.Success)
                    return match;
                _logger?.LogTrace("{0}: Regex for \"{1}\" did not work, trying default regex...", Name, countryCode);
            }

            return _regexMap["default"].Match(streetData);
        }
    }
}
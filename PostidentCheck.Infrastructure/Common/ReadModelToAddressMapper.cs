using Microsoft.Extensions.Logging;
using Postident.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Postident.Infrastructure.Common
{
    public class ReadModelToAddressMapper
    {
        public DefaultNamingMap NamingMap { get; }
        private readonly ILogger<ReadModelToAddressMapper> _logger;
        private const string Name = "ReadModel to Address mapper";

        public ReadModelToAddressMapper(DefaultNamingMap namingMap, ILogger<ReadModelToAddressMapper> logger)
        {
            NamingMap = namingMap;
            _logger = logger;
        }

        private readonly Regex _splitAddressRegex = new(@"^([\w\.\-\, ]+) ([0-9]{1,5})\s?([\w\.\-/]*)$");

        public Address Map(DataPackReadModel dataPack)
        {
            var (street, number) = SplitStreet(dataPack);

            return new Address()
            {
                City = dataPack.City,
                CountryCode = dataPack.CountryCode,
                PostIdent = dataPack.PostIdent,
                Street = street,
                StreetNumber = number,
                ZipCode = dataPack.ZipCode
            };
        }

        public IEnumerable<Address> Map(IEnumerable<DataPackReadModel> dataPacks) => dataPacks.Select(this.Map);

        private (string street, string number) SplitStreet(DataPackReadModel dataPack)
        {
            var result = _splitAddressRegex.Match(dataPack.Street);
            if (!result.Success || result.Groups.Count == 1)
            {
                _logger?.LogWarning("{0}: ID: {1} - could not split street into separate address and number - mapping as street without number.", Name, dataPack.Id);
                return (dataPack.Street.Trim(), string.Empty);
            }

            var streetMatch = NamingMap.TryGetDefaultNameFor(result.Groups[0].Value.Trim(), out var mappedName) ?
                mappedName :
                result.Groups[0].Value.Trim();

            var numberMatch = string.Empty;

            for (var i = 1; i < result.Groups.Count; i++)
            {
                numberMatch += result.Groups[i].Value.Trim();
            }

            return (streetMatch, numberMatch);
        }
    }
}
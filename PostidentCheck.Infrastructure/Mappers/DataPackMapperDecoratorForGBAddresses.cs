using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.Common.Models;
using Postident.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Postident.Infrastructure.Mappers
{
    public class DataPackMapperDecoratorForGBAddresses : IReadModelToDataPackMapper
    {
        private const string Name = "ReadModel to DataPack mapper GB decorator";
        private readonly IReadModelToDataPackMapper _mapper;
        private readonly ILogger<IReadModelToDataPackMapper> _logger;

        public DataPackMapperDecoratorForGBAddresses(IReadModelToDataPackMapper mapper, ILogger<IReadModelToDataPackMapper> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger;
        }

        /// <summary>
        /// <inheritdoc cref="IReadModelToDataPackMapper.Map(DataPackReadModel)"/><br/>
        /// This decorator will also insert a "1" as a street number if the address is in GB and number is empty.
        /// </summary>
        /// <returns><see cref="DataPack"/> created from provided <paramref name="dataPackReadModel"/></returns>
        /// <exception cref="ArgumentNullException">When trying to map <see langword="null"/></exception>
        public DataPack Map(DataPackReadModel dataPackReadModel)
        {
            var mappedDataPack = _mapper.Map(dataPackReadModel);
            if (mappedDataPack.Address.CountryCode.Equals("gb", StringComparison.InvariantCultureIgnoreCase) &&
                string.IsNullOrWhiteSpace(mappedDataPack.Address.StreetNumber))
            {
                _logger?.LogInformation("{0}: ID: {1} - Great Britain address without street number detected - setting street number to default value.", Name, mappedDataPack.Id);
                return new DataPack
                {
                    Address = new Address
                    {
                        StreetNumber = "1",
                        City = mappedDataPack.Address.City,
                        CountryCode = mappedDataPack.Address.CountryCode,
                        Name = mappedDataPack.Address.Name,
                        PostIdent = mappedDataPack.Address.PostIdent,
                        Street = mappedDataPack.Address.Street,
                        ZipCode = mappedDataPack.Address.ZipCode
                    },
                    Carrier = mappedDataPack.Carrier,
                    DataPackChecked = mappedDataPack.DataPackChecked,
                    Email = mappedDataPack.Email,
                    Id = mappedDataPack.Id
                };
            }
            return mappedDataPack;
        }

        /// <summary>
        /// <inheritdoc cref="IReadModelToDataPackMapper.Map(IEnumerable{DataPackReadModel})"/><br/>
        /// This decorator will also insert a "1" as a street number if the address is in GB and number is empty.
        /// </summary>
        /// <param name="dataPackReadModels">Entities from database</param>
        /// <returns>Collection of <see cref="DataPack"/> created from provided <paramref name="dataPackReadModels"/></returns>
        /// <exception cref="ArgumentNullException">When trying to map <see langword="null"/></exception>
        public IEnumerable<DataPack> Map(IEnumerable<DataPackReadModel> dataPackReadModels)
        {
            return dataPackReadModels.Select(Map);
        }
    }
}
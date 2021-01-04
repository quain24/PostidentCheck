using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Interfaces.DHL;
using System;

namespace Postident.Infrastructure.Services.DHL
{
    public class ValidationRequestXmlBuilderFactory : IValidationRequestXmlBuilderFactory
    {
        private readonly IDefaultShipmentValues _defaultShipmentValues;

        public ValidationRequestXmlBuilderFactory(IDefaultShipmentValues defaultShipmentValues)
        {
            _defaultShipmentValues = defaultShipmentValues ?? throw new ArgumentNullException(nameof(defaultShipmentValues));
        }

        public ValidationRequestXmlBuilder CreateInstance() => new(_defaultShipmentValues);
    }
}
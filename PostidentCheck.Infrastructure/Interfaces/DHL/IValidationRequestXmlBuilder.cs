using KeePass.Models;
using Postident.Infrastructure.Services.DHL;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface IValidationRequestXmlBuilder
    {
        IValidationRequestXmlBuilder SetUpAuthorization(Secret secret);

        IValidationRequestXmlBuilder SetUpApiVersion(uint majorRelease = 3, uint minorRelease = 1);

        ISingleShipmentBuilder AddNewShipment();

        IValidationRequestXmlBuilder Reset();

        string Build();
    }
}
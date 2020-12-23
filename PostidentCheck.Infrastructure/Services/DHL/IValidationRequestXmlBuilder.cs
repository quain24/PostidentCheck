using KeePass.Models;

namespace Postident.Infrastructure.Services.DHL
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
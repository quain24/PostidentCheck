using KeePass.Models;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface IValidationRequestXmlBuilder
    {
        IValidationRequestXmlBuilder SetUpAuthorization(Secret secret);

        IValidationRequestXmlBuilder SetUpApiVersion(uint majorRelease = 3, uint minorRelease = 1);

        ISingleShipmentBuilder AddNewShipment();

        IValidationRequestXmlBuilder Reset();

        /// <summary>
        /// Creates xml string representation of Validation request body created from provided data.
        /// </summary>
        /// <returns>XML <see cref="string"/> representation of request body</returns>
        string Build();
    }
}
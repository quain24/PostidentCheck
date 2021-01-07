using Postident.Application.Common.Models;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface IValidationRequestXmlBuilder
    {
        /// <summary>
        /// Provide username and password for xml part of authorization. Those data will put in generated xml.
        /// </summary>
        /// <param name="username">Username used when generating xml</param>
        /// <param name="password">Password used when generating xml</param>
        IValidationRequestXmlBuilder SetUpAuthorization(string username, string password);

        /// <summary>
        /// Mostly unused - can override default api version inside xml request
        /// </summary>
        /// <param name="majorRelease"></param>
        /// <param name="minorRelease"></param>
        /// <returns></returns>
        IValidationRequestXmlBuilder SetUpApiVersion(uint majorRelease = 3, uint minorRelease = 1);

        /// <summary>
        /// Adds a shipment to be checked by api to this builder. Can be used multiple times - builder supports multiple shipments inside one validation request.
        /// </summary>
        /// <param name="id">Id of this shipment - this will be used to match response from api to actual database entry</param>
        /// <param name="receiverAddress">Receiver address</param>
        ISingleShipmentBuilder AddNewShipment(string id, Address receiverAddress);

        /// <summary>
        /// Resets this instance of <see cref="IValidationRequestXmlBuilder"/> to its basic state.
        /// </summary>
        IValidationRequestXmlBuilder Reset();

        /// <summary>
        /// Creates xml string representation of Validation request body created from provided data.
        /// </summary>
        /// <returns>XML <see cref="string"/> representation of request body</returns>
        string Build();
    }
}
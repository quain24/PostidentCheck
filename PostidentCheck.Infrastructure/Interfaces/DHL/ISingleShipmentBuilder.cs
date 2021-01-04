using Postident.Application.Common.Models;
using System;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface ISingleShipmentBuilder
    {
        /// <summary>
        /// Set up a unique <paramref name="id"/> that will enable this app to differentiate received responses.<br/>
        /// <paramref name="id"/> has to correspond to key column of data packs being checked. MANDATORY
        /// </summary>
        /// <param name="id">A key value from data being checked, unique</param>
        ISingleShipmentBuilder SetUpId(string id);

        ISingleShipmentBuilder SetUpDhlServiceType(string type);

        /// <summary>
        /// Theoretical shipping date - should be set to 'today' or a date in near future.
        /// </summary>
        /// <param name="date">Ship-out date of this shipment</param>
        ISingleShipmentBuilder SetUpShippingDate(DateTime date);

        /// <summary>
        /// Set up shippers account number.
        /// </summary>
        /// <param name="accountNumber">Shippers account number</param>
        ISingleShipmentBuilder SetUpAccountNumber(string accountNumber);

        /// <summary>
        /// Set up senders address and name data
        /// </summary>
        /// <param name="address"><see cref="DataPack"/> object containing senders address and naming data</param>
        ISingleShipmentBuilder SetUpSenderData(Address address);

        /// <summary>
        /// Set up receiver address and name data - MANDATORY
        /// </summary>
        /// <param name="address"><see cref="DataPack"/> object containing receivers address and naming data</param>
        ISingleShipmentBuilder SetUpReceiverData(Address address);

        /// <summary>
        /// You can set up custom shipment dimensions - they will be validated by online service.
        /// </summary>
        /// <param name="weightInKg">Shipment weight in kilograms</param>
        /// <param name="lengthInCm">Shipment length in centimeters</param>
        /// <param name="widthInCm">Shipment width in centimeters</param>
        /// <param name="heightInCm">Shipment height in centimeters</param>
        ISingleShipmentBuilder SetUpItemDimensions(uint weightInKg, uint lengthInCm, uint widthInCm,
            uint heightInCm);

        ISingleShipmentBuilder Reset();

        /// <summary>
        /// Finishes and builds a single shipment.
        /// </summary>
        /// <returns>Parent <see cref="IValidationRequestXmlBuilder"/> object</returns>
        /// <exception cref="MissingFieldException">If one or more required properties / fields are missing</exception>
        IValidationRequestXmlBuilder BuildShipment();
    }
}
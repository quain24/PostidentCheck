using System;
using Postident.Infrastructure.Common;

namespace Postident.Infrastructure.Interfaces.DHL
{
    public interface ISingleShipmentBuilder
    {
        /// <summary>
        /// Set up a unique <paramref name="id"/> that will enable this app to differentiate received responses.<br/>
        /// <paramref name="id"/> has to correspond to key column of data packs being checked.
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
        /// <param name="address"><see cref="Address"/> object containing senders address and naming data</param>
        ISingleShipmentBuilder SetUpSenderData(Address address);

        /// <summary>
        /// Set up receiver address and name data - MANDATORY
        /// </summary>
        /// <param name="address"><see cref="Address"/> object containing receivers address and naming data</param>
        ISingleShipmentBuilder SetUpReceiverData(Address address);

        ISingleShipmentBuilder SetUpItemDimensions(uint weightInKg, uint lengthInCm, uint widthInCm,
            uint heightInCm);

        ISingleShipmentBuilder Reset();

        IValidationRequestXmlBuilder BuildShipment();
    }
}
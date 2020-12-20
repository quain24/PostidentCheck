namespace Postident.Core.Enums
{
    /// <summary>
    /// Possible carrier services to choose from
    /// </summary>
    public enum Carrier
    {
        DPD = 2,

        /// <summary>
        /// Delivered by our own employees (in-house transport service)
        /// </summary>
        Self_delivery = 4,

        DHL = 6,
        Schenker = 7,
        DHL_Sperr = 10,
        Pick_up = 11,
        HES = 30,
        DPD_PL = 40
    }
}
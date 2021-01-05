namespace Postident.Application.Common.Models
{
    public class Address
    {
        /// <summary>
        /// Street name or "Packstation" / "Postfiliale"
        /// </summary>
        public string Street { get; init; }

        /// <summary>
        /// Street number or Packstation / Postfiliale number
        /// </summary>
        public string StreetNumber { get; init; } = string.Empty;

        public string Name { get; init; } = "Private";
        public string PostIdent { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string ZipCode { get; init; } = string.Empty;

        /// <summary>
        /// 2-3 letter euro country code (DE)
        /// </summary>
        public string CountryCode { get; init; } = string.Empty;

        public override string ToString()
        {
            return $"{Name} - {CountryCode} - {ZipCode} {City}, {Street} {StreetNumber}."
                   + (string.IsNullOrEmpty(PostIdent) ? string.Empty : $" (Postident: {PostIdent})");
        }
    }
}
using Postident.Core.Enums;

namespace Postident.Core.Entities
{
    public class DataPackReadModel
    {
        public string Id { get; init; }
        public Carrier Carrier { get; init; }
        public string PostIdent { get; init; }
        public string Street { get; init; }
        public string ZipCode { get; init; }
        public string City { get; init; }
        public string CountryCode { get; init; }
        public InfoPackCheckStatus DataPackChecked { get; init; }
    }
}
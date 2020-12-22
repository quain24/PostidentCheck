using Postident.Core.Enums;

namespace Postident.Core.Entities
{
    public class DataPackReadModel
    {
        public string Id { get; set; }
        public Carrier Carrier { get; set; }
        public string PostIdent { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        /// <summary>
        /// -1 - not checked, 0 - checked, contains errors, 1 - valid
        /// </summary>
        public int DataPackChecked { get; set; }
    }
}
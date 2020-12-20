namespace Postident.Core.Entities
{
    public class DataPack
    {
        public string PostIdent { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ErrorInfo { get; set; }
        public int PostIdentChecked { get; set; }
    }
}
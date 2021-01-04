using Postident.Core.Enums;

namespace Postident.Application.Common.Models
{
    public class DataPack
    {
        public string Id { get; init; }
        public Carrier Carrier { get; init; }

        public Address Address { get; init; }

        public InfoPackCheckStatus DataPackChecked { get; init; }
    }
}
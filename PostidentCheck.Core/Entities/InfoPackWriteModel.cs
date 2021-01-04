using Postident.Core.Enums;

namespace Postident.Core.Entities
{
    public class InfoPackWriteModel
    {
        public string Id { get; init; }
        public InfoPackCheckStatus CheckStatus { get; set; }
        public string Message { get; set; }
    }
}
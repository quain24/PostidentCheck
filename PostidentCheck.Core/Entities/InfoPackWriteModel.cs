using Postident.Core.Enums;

namespace Postident.Core.Entities
{
    public class InfoPackWriteModel
    {
        public string Id { get; set; }
        public InfoPackCheckStatus CheckStatus { get; set; }
        public string Message { get; set; }
    }
}
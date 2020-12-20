using System.IO;
using System.Text;

namespace Postident.Infrastructure.Common
{
    /// <summary>
    /// String writer set to UTF8 encoding
    /// </summary>
    public class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
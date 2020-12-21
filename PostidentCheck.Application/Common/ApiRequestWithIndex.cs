using System.Collections.Generic;
using System.Net.Http;

namespace Postident.Application.Common
{
    public class ApiRequestWithIndex
    {
        public List<string> Index { get; init; }
        public HttpRequestMessage Request { get; init; }
    }
}
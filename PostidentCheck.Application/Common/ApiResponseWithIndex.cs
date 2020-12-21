using System.Collections.Generic;
using System.Net.Http;

namespace Postident.Application.Common
{
    public class ApiResponseWithIndex
    {
        public List<string> Index { get; init; }
        public HttpResponseMessage Response { get; init; }
    }
}
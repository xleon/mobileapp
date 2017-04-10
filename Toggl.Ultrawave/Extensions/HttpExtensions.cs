using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Toggl.Ultrawave.Extensions
{
    internal static class HttpExtensions
    {
        public static void AddRange(this HttpRequestHeaders self, IDictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                self.Add(header.Key, header.Value);
            }
        }
    }
}

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Toggl.Ultrawave.Tests.Integration.Helper
{
    public static class AuthorizedRequestBuilder
    {
        public static HttpRequestMessage CreateRequest(string email, string password, string endPoint, HttpMethod method)
        {
            var authString = $"{email}:{password}";
            var authStringBytes = Encoding.UTF8.GetBytes(authString);
            var authHeader = Convert.ToBase64String(authStringBytes);

            var requestMessage = new HttpRequestMessage(method, endPoint);
            requestMessage.Headers.Authorization = 
                new AuthenticationHeaderValue("Basic", authHeader);

            return requestMessage;
        }
    }
}

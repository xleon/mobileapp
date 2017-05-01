using System.Net;
using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    internal sealed class Response : IResponse
    {
        public string RawData { get; }
        public bool IsSuccess { get; }
        public string ContentType { get; }
        public HttpStatusCode StatusCode { get; }

        public Response(string rawData, bool isSuccess, string contentType, HttpStatusCode statusCode)
        {
            Ensure.ArgumentIsNotNull(rawData, nameof(rawData));
            Ensure.ArgumentIsNotNull(contentType, nameof(contentType));

            RawData = rawData;
            IsSuccess = isSuccess;
            ContentType = contentType;
            StatusCode = statusCode;
        }
    }
}
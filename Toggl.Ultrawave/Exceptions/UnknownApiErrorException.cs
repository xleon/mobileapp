using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class UnknownApiErrorException : ApiException
    {
        public HttpStatusCode HttpCode { get; }

        private const string defaultMessage = "The server responded with an unexpected HTTP status code.";

        public UnknownApiErrorException(HttpStatusCode httpCode)
            : this(defaultMessage, httpCode)
        {
        }

        public UnknownApiErrorException(string errorMessage, HttpStatusCode httpCode)
            : base(errorMessage)
        {
            HttpCode = httpCode;
        }
    }
}

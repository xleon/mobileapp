using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class HttpVersionNotSupportedException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.HttpVersionNotSupported;

        private const string defaultMessage = "HTTP version is not supported.";

        public HttpVersionNotSupportedException()
            : this(defaultMessage)
        {
        }

        public HttpVersionNotSupportedException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
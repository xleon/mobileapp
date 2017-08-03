using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class GatewayTimeoutException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.GatewayTimeout;

        private const string defaultMessage = "Bad gateway.";

        public GatewayTimeoutException()
            : this(defaultMessage)
        {
        }

        public GatewayTimeoutException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
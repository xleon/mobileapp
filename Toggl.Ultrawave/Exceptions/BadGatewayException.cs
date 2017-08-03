using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class BadGatewayException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.BadGateway;

        private const string defaultMessage = "Bad gateway.";

        public BadGatewayException()
            : this(defaultMessage)
        {
        }

        public BadGatewayException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

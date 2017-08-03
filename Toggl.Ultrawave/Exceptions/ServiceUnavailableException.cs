using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class ServiceUnavailableException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.ServiceUnavailable;

        private const string defaultMessage = "Service unavailable.";

        public ServiceUnavailableException()
            : this(defaultMessage)
        {
        }

        public ServiceUnavailableException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

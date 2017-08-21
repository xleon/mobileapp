using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class TooManyRequestsException : ClientErrorException
    {
        // HTTP status code 429 "Too Many Redirects" is not included in the System.Net.HttpStatusCode enum.
        public const HttpStatusCode CorrespondingHttpCode = (HttpStatusCode)429;

        private const string defaultMessage = "The rate limiting does not work properly, fix it.";

        public TooManyRequestsException()
            : this(defaultMessage)
        {
        }

        public TooManyRequestsException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

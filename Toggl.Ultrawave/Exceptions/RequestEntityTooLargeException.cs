using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class RequestEntityTooLargeException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.RequestEntityTooLarge;

        private const string defaultMessage = "The payload is too large, split it into batches.";

        public RequestEntityTooLargeException()
            : this(defaultMessage)
        {
        }

        public RequestEntityTooLargeException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

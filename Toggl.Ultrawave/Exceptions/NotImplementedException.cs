using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class NotImplementedException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.NotImplemented;

        private const string defaultMessage = "This feature is not implemented.";

        public NotImplementedException()
            : this(defaultMessage)
        {
        }

        public NotImplementedException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class ForbiddenException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.Forbidden;

        private const string defaultMessage = "User cannot perform this request.";

        public ForbiddenException()
            : this(defaultMessage)
        {
        }

        public ForbiddenException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

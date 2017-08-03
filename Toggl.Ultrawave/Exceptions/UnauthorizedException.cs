using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class UnauthorizedException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.Unauthorized;

        private const string defaultMessage = "User is not authorized to make this request and must enter login again.";

        public UnauthorizedException()
            : this(defaultMessage)
        {
        }

        public UnauthorizedException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class InternalServerErrorException : ServerErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.InternalServerError;

        private const string defaultMessage = "Internal server error.";

        public InternalServerErrorException()
            : this(defaultMessage)
        {
        }

        public InternalServerErrorException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

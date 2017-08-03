using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class BadRequestException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.BadRequest;

        private const string defaultMessage = "The data is not valid or acceptable.";

        public BadRequestException()
            : this(defaultMessage)
        {
        }

        public BadRequestException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    sealed class NotFoundException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.NotFound;

        private const string defaultMessage = "The resource was not found.";

        public NotFoundException()
            : this(defaultMessage)
        {
        }

        public NotFoundException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

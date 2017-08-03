using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public sealed class ApiDeprecatedException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.Gone;

        private const string defaultMessage = "This version of API is deprecated and the client must be updated to an up-to-date version.";

        public ApiDeprecatedException()
            : this(defaultMessage)
        {
        }

        public ApiDeprecatedException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

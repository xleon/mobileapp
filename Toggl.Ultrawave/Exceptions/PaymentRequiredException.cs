using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    class PaymentRequiredException : ClientErrorException
    {
        public const HttpStatusCode CorrespondingHttpCode = HttpStatusCode.PaymentRequired;

        private const string defaultMessage = "Payment is required for this request.";

        public PaymentRequiredException()
            : this(defaultMessage)
        {
        }

        public PaymentRequiredException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

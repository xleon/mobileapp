namespace Toggl.Ultrawave.Exceptions
{
    public class EndpointUnreachableException : ApiException
    {
        private const string defaultErrorMessage = "Api endpoint could not be reached.";

        public EndpointUnreachableException()
            : this(defaultErrorMessage)
        {
        }

        public EndpointUnreachableException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

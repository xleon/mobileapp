namespace Toggl.Ultrawave.Exceptions
{
    public class NoConnectionException : EndpointUnreachableException
    {
        private const string defaultErrorMessage = "No connection found.";

        public NoConnectionException()
            : this(defaultErrorMessage)
        {
        }

        public NoConnectionException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

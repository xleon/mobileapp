namespace Toggl.Ultrawave.Exceptions
{
    public sealed class NoConnectionException : EndpointUnreachableException
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

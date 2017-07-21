namespace Toggl.Ultrawave.Exceptions
{
    public class NotAuthorizedException : ApiException
    {
        public NotAuthorizedException(string message)
            : base(message)
        {
        }
    }
}

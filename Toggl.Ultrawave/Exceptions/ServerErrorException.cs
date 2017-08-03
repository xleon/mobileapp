using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public abstract class ServerErrorException : ApiException
    {
        protected ServerErrorException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

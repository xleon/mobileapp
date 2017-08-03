using System.Net;

namespace Toggl.Ultrawave.Exceptions
{
    public abstract class ClientErrorException : ApiException
    {
        protected ClientErrorException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}

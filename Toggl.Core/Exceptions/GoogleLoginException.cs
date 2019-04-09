using System;
namespace Toggl.Foundation.Exceptions
{
    public sealed class GoogleLoginException : Exception
    {
        public bool LoginWasCanceled { get; }

        public GoogleLoginException(bool loginWasCanceled)
        {
            LoginWasCanceled = loginWasCanceled;
        }
    }
}

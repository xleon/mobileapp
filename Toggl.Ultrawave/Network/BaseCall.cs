using System;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Network
{
    public abstract class BaseCall<T> : ICall<T>
    {
        public bool Executed { get; private set; }

        ~BaseCall()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Task<IApiResponse<T>> Execute()
        {
            if (Executed) throw new InvalidOperationException("You can only execute a call once");
            Executed = true;
            return SafeExecute();
        }

        protected virtual void Dispose(bool disposing) { }

        protected abstract Task<IApiResponse<T>> SafeExecute();
    }
}

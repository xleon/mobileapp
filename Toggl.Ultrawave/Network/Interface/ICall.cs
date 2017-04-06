using System;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Network
{
    public interface ICall<T> : IDisposable
    {
        bool Executed { get; }

        TimeSpan Timeout { get; }

        Task<IApiResponse<T>> Execute();

        ICall<T> WithTimeout(TimeSpan retryCount);
    }
}

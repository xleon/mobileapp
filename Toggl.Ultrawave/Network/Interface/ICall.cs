using System;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Network
{
    public interface ICall<T> : IDisposable
    {
        bool Executed { get; }

        Task<IApiResponse<T>> Execute();
    }
}

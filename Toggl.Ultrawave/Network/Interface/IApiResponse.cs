using System.Net;
using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    public interface IApiResponse<T>
    {
        Either<T, ErrorMessage> Data { get; }

        bool Success { get; }
    }
}

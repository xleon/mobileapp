using Toggl.Networking.Exceptions;

namespace Toggl.Core.Sync.States.Pull
{
    public interface IPersistState : ISyncState<IFetchObservables>
    {
        StateResult<IFetchObservables> Done { get; }
    }
}

using System;

namespace Toggl.Multivac.Models
{
    public interface IClient : IBaseModel
    {
        long WorkspaceId { get; }

        string Name { get; }

        DateTimeOffset At { get; }

        DateTimeOffset? ServerDeletedAt { get; }
    }
}

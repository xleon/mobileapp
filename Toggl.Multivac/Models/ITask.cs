using System;

namespace Toggl.Multivac.Models
{
    public interface ITask : IBaseModel
    {
        string Name { get; }

        long ProjectId { get; }

        long WorkspaceId { get; }

        long? UserId { get; }

        int EstimatedSeconds { get; }

        bool Active { get; }

        DateTimeOffset At { get; }

        int TrackedSeconds { get; }
    }
}

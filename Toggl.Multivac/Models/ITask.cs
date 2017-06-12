using System;

namespace Toggl.Multivac.Models
{
    public interface ITask : IBaseModel
    {
        string Name { get; }

        int ProjectId { get; }

        int WorkspaceId { get; }

        int? UserId { get; }

        int? EstimatedSeconds { get; }

        bool Active { get; }

        DateTimeOffset At { get; }

        int TrackedSeconds { get; }
    }
}

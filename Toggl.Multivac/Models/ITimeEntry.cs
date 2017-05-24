using System;
using System.Collections.Generic;

namespace Toggl.Multivac.Models
{
    public interface ITimeEntry : IBaseModel
    {
        int WorkspaceId { get; }

        int? ProjectId { get; }

        int? TaskId { get; }

        bool Billable { get; }

        DateTimeOffset Start { get; }

        DateTimeOffset? Stop { get; }

        int Duration { get; }

        string Description { get; }

        IList<string> Tags { get; }

        IList<int> TagIds { get; }

        DateTimeOffset At { get; }

        DateTimeOffset? ServerDeletedAt { get; }

        int UserId { get; }

        string CreatedWith { get; }
    }
}

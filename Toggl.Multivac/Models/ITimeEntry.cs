using System;
using System.Collections.Generic;

namespace Toggl.Multivac.Models
{
    public interface ITimeEntry : IBaseModel
    {
        long WorkspaceId { get; }

        long? ProjectId { get; }

        long? TaskId { get; }

        bool Billable { get; }

        DateTimeOffset Start { get; }

        DateTimeOffset? Stop { get; }

        string Description { get; }

        IList<string> Tags { get; }

        IList<int> TagIds { get; }

        DateTimeOffset At { get; }

        DateTimeOffset? ServerDeletedAt { get; }

        long UserId { get; }

        string CreatedWith { get; }
    }
}

using System;
using Toggl.Multivac.Models;
using System.Collections.Generic;

namespace Toggl.Ultrawave.Models
{
    public sealed partial class TimeEntry : ITimeEntry
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public long? ProjectId { get; set; }

        public long? TaskId { get; set; }

        public bool Billable { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Stop { get; set; }

        public int Duration { get; set; }

        public string Description { get; set; }

        public IList<string> Tags { get; set; }

        public IList<int> TagIds { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }
        
        public long UserId { get; set; }

        public string CreatedWith { get; set; }
    }
}

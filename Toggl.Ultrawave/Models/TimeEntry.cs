using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;
using System.Collections.Generic;

namespace Toggl.Ultrawave
{
    public sealed class TimeEntry : ITimeEntry
    {
        public int Id { get; set; }

        public int WorkspaceId { get; set; }

        public int? ProjectId { get; set; }

        public int? TaskId { get; set; }

        public bool Billable { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Stop { get; set; }

        public int Duration { get; set; }

        public string Description { get; set; }

        public IList<string> Tags { get; set; }

        public IList<int> TagIds { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }
        
        public int UserId { get; set; }

        public string CreatedWith { get; set; }
    }
}

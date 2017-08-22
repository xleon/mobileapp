using System;
using Toggl.Multivac.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Toggl.Ultrawave.Models
{
    internal sealed partial class TimeEntry : ITimeEntry
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public long? ProjectId { get; set; }

        public long? TaskId { get; set; }

        public bool Billable { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Stop { get; set; }

        public string Description { get; set; }

        [JsonProperty("tags")]
        public IList<string> TagNames { get; set; }

        public IList<long> TagIds { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }
        
        public long UserId { get; set; }

        public string CreatedWith { get; set; }
    }
}

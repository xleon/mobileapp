using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    internal partial class Task : ITask
    {
        public long Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("pid")]
        public long ProjectId { get; set; }

        [JsonProperty("wid")]
        public long WorkspaceId { get; set; }

        [JsonProperty("uid")]
        public long? UserId { get; set; }

        public long EstimatedSeconds { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        public long TrackedSeconds { get; set; }
    }
}

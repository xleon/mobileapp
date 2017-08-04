using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public partial class Task : ITask
    {
        public int Id { get; set; }

        public string Name { get; set; }

        [JsonProperty("pid")]
        public int ProjectId { get; set; }

        [JsonProperty("wid")]
        public int WorkspaceId { get; set; }

        [JsonProperty("uid")]
        public int? UserId { get; set; }

        public int EstimatedSeconds { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        public int TrackedSeconds { get; set; }
    }
}

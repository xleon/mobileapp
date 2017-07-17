using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave
{
    public sealed class Project : IProject
    {
        public int Id { get; set; }

        [JsonProperty("wid")]
        public int WorkspaceId { get; set; }

        [JsonProperty("cid")]
        public int? ClientId { get; set; }

        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public string Color { get; set; }

        public bool Billable { get; set; }

        public bool Template { get; set; }

        public bool AutoEstimates { get; set; }

        public int? EstimatedHours { get; set; }

        public int? Rate { get; set; }

        public string Currency { get; set; }

        public int ActualHours { get; set; }
    }
}

using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Models
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

        [IgnoreWhenPosting]
        public DateTimeOffset? ServerDeletedAt { get; set; }

        public string Color { get; set; }

        [IgnoreWhenPosting]
        public bool? Billable { get; set; }

        [IgnoreWhenPosting]
        public bool? Template { get; set; }

        [IgnoreWhenPosting]
        public bool? AutoEstimates { get; set; }

        [IgnoreWhenPosting]
        public int? EstimatedHours { get; set; }

        [IgnoreWhenPosting]
        public int? Rate { get; set; }

        [IgnoreWhenPosting]
        public string Currency { get; set; }

        [IgnoreWhenPosting]
        public int? ActualHours { get; set; }
    }
}

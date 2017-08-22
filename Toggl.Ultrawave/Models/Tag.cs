using System;
using Toggl.Multivac.Models;
using Newtonsoft.Json;

namespace Toggl.Ultrawave.Models
{
    internal sealed partial class Tag : ITag
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }

        [JsonConstructor]
        public Tag(long id, long workspaceId, string name, DateTimeOffset? at)
        {
            Id = id;
            WorkspaceId = workspaceId;
            Name = name;
            At = at ?? new DateTimeOffset(DateTime.UtcNow);
        }
    }
}

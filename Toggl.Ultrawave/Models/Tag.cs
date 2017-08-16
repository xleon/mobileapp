using System;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public sealed partial class Tag : ITag
    {
        public long Id { get; set; }

        public long WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }
    }
}

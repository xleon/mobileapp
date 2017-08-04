using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public sealed partial class Tag : ITag
    {
        public int Id { get; set; }

        public int WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }
    }
}

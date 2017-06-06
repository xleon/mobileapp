using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public sealed class Client : IClient
    {
        public int Id { get; set; }

        [JsonProperty("wid")]
        public int WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }
    }
}

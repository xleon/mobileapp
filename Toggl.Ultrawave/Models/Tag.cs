using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public sealed class Tag : ITag
    {
        public int Id { get; set; }

        [JsonProperty("wid")]
        public int WorkspaceId { get; set; }

        public string Name { get; set; }
    }
}

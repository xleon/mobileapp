using Newtonsoft.Json;

namespace Toggl.Ultrawave
{
    public sealed class Tag
    {
        public int Id { get; set; }

        [JsonProperty("wid")]
        public int WorkspaceId { get; set; }

        public string Name { get; set; }
    }
}

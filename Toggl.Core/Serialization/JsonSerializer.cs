using Newtonsoft.Json;

namespace Toggl.Core.Serialization
{
    public sealed class JsonSerializer : IJsonSerializer
    {
        public T Deserialize<T>(string json)
            => JsonConvert.DeserializeObject<T>(json);
    }
}

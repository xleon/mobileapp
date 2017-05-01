using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Toggl.Ultrawave.Network
{
    internal sealed class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings settings;

        public JsonSerializer()
        {
            settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };
        }

        public Task<T> Deserialize<T>(string json)
            => Task.Run(() => JsonConvert.DeserializeObject<T>(json, settings));

        public Task<string> Serialize<T>(T data)
            => Task.Run(() => JsonConvert.SerializeObject(data, Formatting.None, settings));
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static Toggl.Ultrawave.Serialization.SerializationReason;

namespace Toggl.Ultrawave.Serialization
{
    internal class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings defaultSettings = SerializerSettings.For<DefaultContractResolver>();

        private readonly JsonSerializerSettings postSettings = 
            SerializerSettings.For<IgnoreAttributeContractResolver<IgnoreWhenPostingAttribute>>();

        public T Deserialize<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, defaultSettings);

        public string Serialize<T>(T data, SerializationReason reason = Default)
            => JsonConvert.SerializeObject(data, Formatting.None, getSettings(reason));

        private JsonSerializerSettings getSettings(SerializationReason reason)
        {
            switch (reason)
            {
                case Post:
                    return postSettings;
                default:
                    return defaultSettings;
            }
        }
    }
}

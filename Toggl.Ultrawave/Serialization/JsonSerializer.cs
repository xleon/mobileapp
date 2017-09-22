using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Toggl.Multivac.Models;
using static Toggl.Ultrawave.Serialization.SerializationReason;

namespace Toggl.Ultrawave.Serialization
{
    internal sealed class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings defaultSettings = SerializerSettings.For(new DefaultContractResolver());

        private JsonSerializerSettings postSettings(IWorkspaceFeatureCollection features)
            => SerializerSettings.For(new FilterPropertiesContractResolver(
                new List<IPropertiesFilter>
                {
                    new IgnoreAttributeFilter<IgnoreWhenPostingAttribute>(),
                    new RequiresFeatureAttributeFilter(features)
                }));

        public T Deserialize<T>(string json)
            => JsonConvert.DeserializeObject<T>(json, defaultSettings);

        public string Serialize<T>(T data, SerializationReason reason = Default, IWorkspaceFeatureCollection features = null)
            => JsonConvert.SerializeObject(data, Formatting.None, getSettings(reason, features));

        private JsonSerializerSettings getSettings(SerializationReason reason, IWorkspaceFeatureCollection features)
        {
            switch (reason)
            {
                case Post:
                    return postSettings(features);
                default:
                    return defaultSettings;
            }
        }
    }
}

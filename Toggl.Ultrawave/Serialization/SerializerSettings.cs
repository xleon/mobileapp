using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Toggl.Ultrawave.Serialization
{
    internal static class SerializerSettings
    {
        public static JsonSerializerSettings For<TContractResolver>()
            where TContractResolver : DefaultContractResolver, new()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new TContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()    
                }
            };
        }
    }
}

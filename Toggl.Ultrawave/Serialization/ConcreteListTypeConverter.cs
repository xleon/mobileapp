using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Toggl.Ultrawave.Serialization
{
    class ConcreteListTypeConverter<TConcrete, TInterface> : JsonConverter
        where TConcrete : TInterface
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
            => serializer.Serialize(writer, value);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
            => serializer.Deserialize<List<TConcrete>>(reader)
                .Cast<TInterface>()
                .ToList();

        public override bool CanConvert(Type objectType)
            => objectType.GetGenericTypeDefinition() == typeof(List<TConcrete>);
    }
}

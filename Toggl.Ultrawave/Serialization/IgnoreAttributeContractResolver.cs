using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Toggl.Ultrawave.Serialization
{
    internal sealed class IgnoreAttributeContractResolver<TIgnoredAttribute> : DefaultContractResolver
        where TIgnoredAttribute : IgnoreSerializationAttribute
{
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            foreach (JsonProperty property in properties)
            {
                var attributes = property.AttributeProvider.GetAttributes(typeof(TIgnoredAttribute), false);
                if (attributes.Any()) 
                    property.ShouldSerialize = _ => false;
            }

            return properties;
        } 
    }
}

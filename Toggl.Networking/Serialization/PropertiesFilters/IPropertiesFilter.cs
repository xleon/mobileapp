using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Toggl.Ultrawave.Serialization
{
    public interface IPropertiesFilter
    {
        IList<JsonProperty> Filter(IList<JsonProperty> properties);
    }
}

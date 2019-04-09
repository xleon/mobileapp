using System.Collections.Generic;
using Newtonsoft.Json.Serialization;

namespace Toggl.Networking.Serialization
{
    public interface IPropertiesFilter
    {
        IList<JsonProperty> Filter(IList<JsonProperty> properties);
    }
}

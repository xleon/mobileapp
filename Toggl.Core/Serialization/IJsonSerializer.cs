using System;

namespace Toggl.Core.Serialization
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string json);
    }
}

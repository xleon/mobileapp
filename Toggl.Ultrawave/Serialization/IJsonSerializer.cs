using System.Threading.Tasks;

namespace Toggl.Ultrawave.Serialization
{
    internal interface IJsonSerializer
    {
        Task<T> Deserialize<T>(string json);

        Task<string> Serialize<T>(T data, SerializationReason reason);
    }
}

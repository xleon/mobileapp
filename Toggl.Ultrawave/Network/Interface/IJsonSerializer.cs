using System.Threading.Tasks;

namespace Toggl.Ultrawave.Network
{
    internal interface IJsonSerializer
    {
        Task<string> Serialize<T>(T data);

        Task<T> Deserialize<T>(string json);
    }
}

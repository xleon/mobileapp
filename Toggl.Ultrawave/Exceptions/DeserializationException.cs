using System;
namespace Toggl.Ultrawave.Exceptions
{
    public sealed class DeserializationException<T> : ApiException
    {
        private const string errorMessage = "An error occured while trying to deserialize type {0}. Check the Json property for more info.";

        public string Json { get; set; }

        public DeserializationException(string json)
            : base(string.Format(errorMessage, typeof(T).Name))
        {
            Json = json;
        }
    }
}

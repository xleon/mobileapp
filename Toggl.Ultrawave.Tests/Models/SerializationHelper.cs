using FluentAssertions;
using Toggl.Ultrawave.Serialization;

namespace Toggl.Ultrawave.Tests
{
    public static class SerializationHelper
    {
        private static readonly JsonSerializer serializer = new JsonSerializer();

        internal static void CanBeDeserialized<T>(string validJson, T validObject)
        {
            var actual = serializer.Deserialize<T>(validJson);

            actual.Should().NotBeNull();
            actual.ShouldBeEquivalentTo(validObject);
        }

        internal static void CanBeSerialized<T>(string validJson, T validObject)
        {
            var actualJson = serializer.Serialize(validObject);

            actualJson.Should().Be(validJson);
        }
    }
}

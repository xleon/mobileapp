using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Serialization;
using Xunit;

namespace Toggl.Ultrawave.Tests
{
    public abstract class BaseModelTests<T>
    {
        private readonly JsonSerializer serializer = new JsonSerializer();

        protected abstract string ValidJson { get; }

        protected abstract T ValidObject { get; }

        [Fact]
        public void CanBeDeserialized()
        {
            var actual = serializer.Deserialize<T>(ValidJson);

            actual.Should().NotBeNull();
            actual.ShouldBeEquivalentTo(ValidObject);
        }

        [Fact]
        public void CanBeSerialized()
        {
            var actualJson = serializer.Serialize(ValidObject);

            actualJson.Should().Be(ValidJson);
        }
    }
}

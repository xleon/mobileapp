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
        public async Task CanBeDeserialized()
        {
            var actual = await serializer.Deserialize<T>(ValidJson);

            actual.Should().NotBeNull();
            actual.ShouldBeEquivalentTo(ValidObject);
        }

        [Fact]
        public async Task CanBeSerialized()
        {
            var actualJson = await serializer.Serialize(ValidObject);

            actualJson.Should().Be(ValidJson);
        }
    }
}

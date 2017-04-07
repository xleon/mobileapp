using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests.Network
{
    public class JsonSerializerTests
    {
        private class TestModel
        {
            public string FooBar { get; set; }
        }

        public class TheSerializeMethod
        {
            [Fact]
            public async Task CreatesSnakeCasedJson()
            {
                var testObject = new TestModel { FooBar = "Foo" };
                const string expectedJson = "{\"foo_bar\":\"Foo\"}";

                var jsonSerializer = new JsonSerializer();
                var actual = await jsonSerializer.Serialize(testObject);

                actual.Should().Be(expectedJson);
            }
        }

        public class TheDeserializeMethod
        {
            [Fact]
            public async Task ExpectsSnakeCasedJson()
            {
                const string testJson = "{\"foo_bar\":\"Foo\"}";
                var expectedObject = new TestModel { FooBar = "Foo" };

                var jsonSerializer = new JsonSerializer();
                var actual = await jsonSerializer.Deserialize<TestModel>(testJson);

                actual.FooBar.Should().Be(expectedObject.FooBar);
            }
        }
    }
}

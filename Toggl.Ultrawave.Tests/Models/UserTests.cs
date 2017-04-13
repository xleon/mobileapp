using Xunit;
using Toggl.Ultrawave.Network;
using FluentAssertions;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Tests.Models
{
    public class UserTests
    {
        public class TheUserModel
        {
            private readonly IJsonSerializer serializer = new JsonSerializer();

            private const string validJson = "{\"id\":9000,\"api_token\":\"1971800d4d82861d8f2c1651fea4d212\",\"default_wid\":777,\"email\":\"johnt@swift.com\",\"fullname\":\"John Swift\",\"timeofday_format\":\"h:mm A\",\"date_format\":\"MM/DD/YYYY\",\"store_start_and_stop_time\":true,\"beginning_of_week\":0,\"language\":\"en_US\",\"image_url\":\"https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png\",\"sidebar_piechart\":false,\"at\":\"2013-03-06T12:18:42+00:00\",\"retention\":9,\"record_timeline\":true,\"render_timeline\":true,\"timeline_enabled\":true,\"timeline_experiment\":true}";

            private readonly User validUser = new User
            {
                Id = 9000,
                ApiToken = "1971800d4d82861d8f2c1651fea4d212",
                DefaultWorkspaceId = 777,
                Email = "johnt@swift.com",
                Fullname = "John Swift",
                TimeOfDayFormat = "h:mm A",
                DateFormat = "MM/DD/YYYY",
                StoreStartAndStopTime = true,
                BeginningOfWeek = 0,
                Language = "en_US",
                ImageUrl = "https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png",
                SidebarPiechart = false,
                At = "2013-03-06T12:18:42+00:00",
                Retention = 9,
                RecordTimeline = true,
                RenderTimeline = true,
                TimelineEnabled = true,
                TimelineExperiment = true
            };

            [Fact]
            public async Task CanBeDeserialized()
            {
                var actual = await serializer.Deserialize<User>(validJson);

                actual.Should().NotBeNull();
                actual.ShouldBeEquivalentTo(validUser);
            }

            [Fact]
            public async Task CanBeSerialized()
            {
                var actualJson = await serializer.Serialize(validUser);

                actualJson.Should().Be(validJson);
            }
        }

    }
}

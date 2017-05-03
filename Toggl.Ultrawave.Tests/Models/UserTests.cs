using System;

namespace Toggl.Ultrawave.Tests.Models
{
    public class UserTests
    {
        public class TheUserModel : BaseModelTests<User>
        {
            protected override string ValidJson =>
                "{\"id\":9000,\"api_token\":\"1971800d4d82861d8f2c1651fea4d212\",\"default_workspace_id\":777,\"email\":\"johnt@swift.com\",\"fullname\":\"John Swift\",\"timeofday_format\":\"h:mm A\",\"date_format\":\"MM/DD/YYYY\",\"store_start_and_stop_time\":true,\"beginning_of_week\":0,\"language\":\"en_US\",\"image_url\":\"https://www.toggl.com/system/avatars/9000/small/open-uri20121116-2767-b1qr8l.png\",\"sidebar_piechart\":false,\"at\":\"2013-03-06T12:18:42+00:00\",\"retention\":9,\"record_timeline\":true,\"render_timeline\":true,\"timeline_enabled\":true,\"timeline_experiment\":true}";

            protected override User ValidObject => new User
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
                At = new DateTimeOffset(2013, 3, 6, 12, 18,42, TimeSpan.Zero),
                Retention = 9,
                RecordTimeline = true,
                RenderTimeline = true,
                TimelineEnabled = true,
                TimelineExperiment = true
            };
        }
    }
}

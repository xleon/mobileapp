using System;
using System.Collections.Generic;

namespace Toggl.Ultrawave.Tests.Models
{
    public class TimeEntryTests
    {
        public class TheTimeEntryModel : BaseModelTests<TimeEntry>
        {
            protected override string ValidJson
                => "{\"id\":525144694,\"workspace_id\":1414373,\"project_id\":3178352,\"task_id\":null,\"billable\":false,\"start\":\"2017-04-25T19:34:39+00:00\",\"stop\":null,\"duration\":-1493148879,\"description\":\"Some short description\",\"tags\":[\"one\",\"test\",\"two\"],\"tag_ids\":[313040,3129041,319042],\"at\":\"2017-04-25T20:12:27+00:00\",\"server_deleted_at\":null,\"user_id\":0,\"created_with\":\"SomeApp\"}";

            protected override TimeEntry ValidObject => new TimeEntry
            {
                Id = 525144694,
                WorkspaceId = 1414373,
                ProjectId = 3178352,
                TaskId = null,
                Billable = false,
                Start = new DateTimeOffset(2017, 4, 25, 19, 34, 39, TimeSpan.Zero),
                Stop = null,
                Duration = -1493148879,
                Description = "Some short description",
                Tags = new string[] { "one", "test", "two" },
                TagIds = new int[] { 313040, 3129041, 319042 },
                At = new DateTimeOffset(2017, 4, 25, 20, 12, 27, TimeSpan.Zero),
                ServerDeletedAt = null,
                UserId = 0,
                CreatedWith = "SomeApp"
            };
        }
    }
}

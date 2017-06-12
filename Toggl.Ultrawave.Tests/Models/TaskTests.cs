using System;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.Tests.Models
{
    public class TaskTests : BaseModelTests<Task>
    {
        protected override string ValidJson
            => "{\"id\":79,\"name\":\"new task\",\"pid\":1,\"wid\":56,\"uid\":null,\"estimated_seconds\":13,\"active\":true,\"at\":\"2017-01-01T02:03:00+00:00\",\"tracked_seconds\":12}";

        protected override Task ValidObject => new Task
        {
            Id = 79,
            Name = "new task",
            ProjectId = 1,
            WorkspaceId = 56,
            UserId = null,
            EstimatedSeconds = 13,
            Active = true,
            At = new DateTimeOffset(2017, 1, 1, 2, 3, 0, TimeSpan.Zero),
            TrackedSeconds = 12
        };
    }
}

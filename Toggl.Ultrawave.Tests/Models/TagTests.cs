using System;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.Tests.Models
{
    public class TagTests : BaseModelTests<Tag>
    {
        protected override string ValidJson
            => "{\"id\":2024667,\"workspace_id\":424213,\"name\":\"mobile\",\"at\":\"2014-04-25T10:10:13+00:00\"}";

        protected override Tag ValidObject => new Tag
        {
            Id = 2024667,
            WorkspaceId = 424213,
            Name = "mobile",
            At = new DateTimeOffset(2014, 04, 25, 10, 10, 13, TimeSpan.Zero)
        };
    }
}

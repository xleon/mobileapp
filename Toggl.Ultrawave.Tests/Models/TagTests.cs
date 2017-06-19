using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.Tests.Models
{
    public class TagTests : BaseModelTests<Tag>
    {
        protected override string ValidJson
            => "{\"id\":2024667,\"workspace_id\":424213,\"name\":\"mobile\"}";

        protected override Tag ValidObject => new Tag
        {
            Id = 2024667,
            WorkspaceId = 424213,
            Name = "mobile"
        };
    }
}

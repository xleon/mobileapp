using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Networking.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class ITagExtensions
    {
        public static ITag With(
            this ITag tag,
            New<long> workspaceId = default(New<long>))
            => new Tag
            {
                Id = tag.Id,
                ServerDeletedAt = tag.ServerDeletedAt,
                At = tag.At,
                Name = tag.Name,
                WorkspaceId = workspaceId.ValueOr(tag.WorkspaceId)
            };
    }
}

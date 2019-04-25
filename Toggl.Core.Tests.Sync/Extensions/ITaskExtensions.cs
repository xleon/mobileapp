using Toggl.Shared;
using Toggl.Shared.Models;
using Toggl.Networking.Models;

namespace Toggl.Core.Tests.Sync.Extensions
{
    public static class ITaskExtensions
    {
        public static ITask With(
            this ITask task,
            New<long> workspaceId = default(New<long>),
            New<long> projectId = default(New<long>),
            New<long?> userId = default(New<long?>))
            => new Task
            {
                Id = task.Id,
                At = task.At,
                Name = task.Name,
                ProjectId = projectId.ValueOr(task.ProjectId),
                WorkspaceId = workspaceId.ValueOr(task.WorkspaceId),
                UserId = userId.ValueOr(task.UserId),
                EstimatedSeconds = task.EstimatedSeconds,
                Active = task.Active,
                TrackedSeconds = task.TrackedSeconds
            };
    }
}

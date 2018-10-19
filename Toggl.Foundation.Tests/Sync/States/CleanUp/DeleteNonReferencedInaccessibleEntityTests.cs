using System.Linq;
using Toggl.Foundation.Models.Interfaces;
using Toggl.Foundation.Tests.Mocks;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Tests.Sync.States.CleanUp
{
    public class DeleteNonReferencedInaccessibleEntityTests
    {
        protected IThreadSafeWorkspace GetWorkspace(long id, bool isInaccessible)
            => new MockWorkspace
            {
                Id = id,
                IsInaccessible = isInaccessible
            };

        protected IThreadSafeTimeEntry GetTimeEntry(
            long id,
            IThreadSafeWorkspace workspace,
            SyncStatus syncStatus,
            IThreadSafeTag[] tags = null,
            IThreadSafeProject project = null,
            IThreadSafeTask task = null)
            => new MockTimeEntry
            {
                Id = id,
                Workspace = workspace,
                WorkspaceId = workspace.Id,
                Tags = tags,
                TagIds = tags?.Select(tag => tag.Id),
                Project = project,
                ProjectId = project?.Id,
                Task = task,
                TaskId = task?.Id,
                SyncStatus = syncStatus
            };

        protected IThreadSafeTag GetTag(long id, IThreadSafeWorkspace workspace, SyncStatus syncStatus)
            => new MockTag
            {
                Id = id,
                Workspace = workspace,
                WorkspaceId = workspace.Id,
                SyncStatus = syncStatus
            };

        protected IThreadSafeProject GetProject(long id, IThreadSafeWorkspace workspace, SyncStatus syncStatus)
            => new MockProject
            {
                Id = id,
                Workspace = workspace,
                WorkspaceId = workspace.Id,
                SyncStatus = syncStatus
            };

        protected IThreadSafeTask GetTask(long id, IThreadSafeWorkspace workspace, IThreadSafeProject project, SyncStatus syncStatus)
            => new MockTask
            {
                Id = id,
                Workspace = workspace,
                WorkspaceId = workspace.Id,
                Project = project,
                ProjectId = project.Id,
                SyncStatus = syncStatus
            };
    }
}

using Toggl.Core.Extensions;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.Autocomplete.Suggestions
{
    public sealed class TaskSuggestion : AutocompleteSuggestion
    {
        public long TaskId { get; }

        public string Name { get; }

        public long ProjectId { get; }

        public string ProjectName { get; }

        public string ProjectColor { get; }

        public TaskSuggestion(IThreadSafeTask task)
        {
            TaskId = task.Id;
            Name = task.Name;
            ProjectId = task.ProjectId;
            WorkspaceId = task.WorkspaceId;
            ProjectName = task.Project?.DisplayName() ?? "";
            ProjectColor = task.Project?.DisplayColor() ?? "";
        }

        public override int GetHashCode() 
            => HashCode.From(TaskId, ProjectId, Name);
    }
}

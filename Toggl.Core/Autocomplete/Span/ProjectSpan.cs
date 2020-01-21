using System;
using Toggl.Core.Models.Interfaces;

namespace Toggl.Core.Autocomplete.Span
{
    public sealed class ProjectSpan : ISpan
    {
        public long ProjectId { get; set; }

        public string ProjectColor { get; set; }

        public string ProjectName { get; set; }

        public long? TaskId { get; set; }

        public string TaskName { get; set; }

        public ProjectSpan(long projectId, string projectName, string projectColor)
        {
            ProjectId = projectId;
            ProjectName = projectName;
            ProjectColor = projectColor;
        }

        public ProjectSpan(long projectId, string projectName, string projectColor, long? taskId, string taskName)
        {
            TaskId = taskId;
            TaskName = taskName;
            ProjectId = projectId;
            ProjectName = projectName;
            ProjectColor = projectColor;
        }

        public ProjectSpan(IThreadSafeProject project)
            : this(project.Id, project.Name, project.Color)
        {
        }

        public ProjectSpan(IThreadSafeTask task)
            : this(task.Project, task)
        {
        }

        private ProjectSpan(IThreadSafeProject project, IThreadSafeTask task)
            : this(task.ProjectId, project.Name, project.Color, task.Id, task.Name)
        {
        }

        public bool Equals(ISpan other)
            => other is ProjectSpan projectSpanOther &&
               ProjectId == projectSpanOther.ProjectId &&
               ProjectColor == projectSpanOther.ProjectColor &&
               ProjectName == projectSpanOther.ProjectName &&
               TaskId == projectSpanOther.TaskId &&
               TaskName == projectSpanOther.TaskName;

        public override bool Equals(object obj)
            => Equals(obj as ISpan);

        public override int GetHashCode()
            => HashCode.Combine(ProjectId, ProjectColor, ProjectName, TaskId, TaskName);
    }
}

using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Models
{
    internal sealed class SinceParameters : ISinceParameters
    {
        public DateTimeOffset? Workspaces { get; }
        public DateTimeOffset? Tags { get; }
        public DateTimeOffset? Clients { get; }
        public DateTimeOffset? Projects { get; }
        public DateTimeOffset? Tasks { get; }
        public DateTimeOffset? TimeEntries { get; }

        public SinceParameters(
            ISinceParameters since,
            DateTimeOffset? workspaces = null,
            DateTimeOffset? tags = null,
            DateTimeOffset? clients = null,
            DateTimeOffset? projects = null,
            DateTimeOffset? tasks = null,
            DateTimeOffset? timeEntries = null)
        {
            Workspaces = workspaces ?? since?.Workspaces;
            Tags = tags ?? since?.Tags;
            Clients = clients ?? since?.Clients;
            Projects = projects ?? since?.Projects;
            Tasks = tasks ?? since?.Tasks;
            TimeEntries = timeEntries ?? since?.TimeEntries;
        }
    }
}

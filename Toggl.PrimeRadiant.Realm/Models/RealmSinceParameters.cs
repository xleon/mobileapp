using System;
using Realms;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm.Models
{
    internal class RealmSinceParameters : RealmObject, ISinceParameters
    {
        public void SetValuesFrom(ISinceParameters parameters)
        {
            Workspaces = parameters.Workspaces;
            Tags = parameters.Tags;
            Clients = parameters.Clients;
            Projects = parameters.Projects;
            Tasks = parameters.Tasks;
            TimeEntries = parameters.TimeEntries;
        }

        public DateTimeOffset? Workspaces { get; set; }
        public DateTimeOffset? Tags { get; set; }
        public DateTimeOffset? Clients { get; set; }
        public DateTimeOffset? Projects { get; set; }
        public DateTimeOffset? Tasks { get; set; }
        public DateTimeOffset? TimeEntries { get; set; }
    }
}

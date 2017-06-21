using System;
using Realms;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmTask : RealmObject, IDatabaseTask
    {
        public string Name { get; set; }

        public int ProjectId { get; set; }

        public int WorkspaceId { get; set; }

        public int? UserId { get; set; }

        public int EstimatedSeconds { get; set; }

        public bool Active { get; set; }

        public DateTimeOffset At { get; set; }

        public int TrackedSeconds { get; set; }
    }
}

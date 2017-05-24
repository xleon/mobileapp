using System;
using Realms;
using Toggl.PrimeRadiant.Models;
using System.Collections.Generic;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmTimeEntry : RealmObject, IDatabaseTimeEntry
    {
        public int WorkspaceId { get; }

        public int? ProjectId { get; }

        public int? TaskId { get; }

        public bool Billable { get; }

        public DateTimeOffset Start { get; }

        public DateTimeOffset? Stop { get; }

        public int Duration { get; }

        public string Description { get; }

        public IList<string> Tags { get; }

        public IList<int> TagIds { get; }

        public DateTimeOffset At { get; }

        public DateTimeOffset? ServerDeletedAt { get; }

        public int UserId { get; }

        public string CreatedWith { get; }
    }
}

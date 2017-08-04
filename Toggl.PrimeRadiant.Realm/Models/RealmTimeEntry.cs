using System;
using Realms;
using Toggl.PrimeRadiant.Models;
using System.Collections.Generic;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmTimeEntry : RealmObject, IDatabaseTimeEntry
    {
        public int WorkspaceId { get; set; }

        public int? ProjectId { get; set; }

        public int? TaskId { get; set; }

        public bool Billable { get; set; }

        public DateTimeOffset Start { get; set; }

        public DateTimeOffset? Stop { get; set; }

        public int Duration { get; set; }

        public string Description { get; set; }

        public IList<string> Tags { get; set; }

        public IList<int> TagIds { get; set; }

        public DateTimeOffset At { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public int UserId { get; set; }

        public string CreatedWith { get; set; }

        public bool IsDeleted { get; set; }
    }
}

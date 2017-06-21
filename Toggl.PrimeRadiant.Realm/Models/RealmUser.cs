using System;
using Realms;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmUser : RealmObject, IDatabaseUser
    {
        public string ApiToken { get; set; }

        public DateTimeOffset At { get; set; }

        public int BeginningOfWeek { get; set; }

        public string DateFormat { get; set; }

        public int DefaultWorkspaceId { get; set; }

        public string Email { get; set; }

        public string Fullname { get; set; }

        public string ImageUrl { get; set; }

        public string Language { get; set; }

        public bool RecordTimeline { get; set; }

        public bool RenderTimeline { get; set; }

        public int Retention { get; set; }

        public bool SidebarPiechart { get; set; }

        public bool StoreStartAndStopTime { get; set; }

        public bool TimelineEnabled { get; set; }

        public bool TimelineExperiment { get; set; }

        public string TimeOfDayFormat { get; set; }
    }
}

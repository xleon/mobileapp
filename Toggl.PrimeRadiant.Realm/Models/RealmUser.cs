using System;
using Realms;
using Toggl.Multivac;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmUser : RealmObject, IDatabaseUser
    {
        public string ApiToken { get; set; }

        public DateTimeOffset At { get; set; }

        public string DateFormat { get; set; }

        public long DefaultWorkspaceId { get; set; }

        //Realm doesn't support enums 
        [Ignored]
        public BeginningOfWeek BeginningOfWeek
        {
            get => (BeginningOfWeek)BeginningOfWeekInt;
            set => BeginningOfWeekInt = (int)value;
        }

        public int BeginningOfWeekInt { get; set; }
        
        public string Email { get; set; }

        public string Fullname { get; set; }

        public string ImageUrl { get; set; }

        public string Language { get; set; }

        public string TimeOfDayFormat { get; set; }
    }
}

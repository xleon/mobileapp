using System;
using Newtonsoft.Json;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public sealed class User : IUser
    {
        public int Id { get; set; }

        public string ApiToken { get; set; }
        
        public int DefaultWorkspaceId { get; set; }

        public string Email { get; set; }

        public string Fullname { get; set; }

        //BACKEND Y U STEP ON SNEK??
        [JsonProperty("timeofday_format")]
        public string TimeOfDayFormat { get; set; }

        public string DateFormat { get; set; }

        public bool StoreStartAndStopTime { get; set; }

        public int BeginningOfWeek { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public bool SidebarPiechart { get; set; }

        public DateTimeOffset At { get; set; }
      
        public int Retention { get; set; }

        public bool RecordTimeline { get; set; }

        public bool RenderTimeline { get; set; }

        public bool TimelineEnabled { get; set; }

        public bool TimelineExperiment { get; set; }
    }
}

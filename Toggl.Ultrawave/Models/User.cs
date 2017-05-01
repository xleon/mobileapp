using Newtonsoft.Json;

namespace Toggl.Ultrawave
{
    public sealed class User
    {
        public int Id { get; set; }

        public string ApiToken { get; set; }

        [JsonProperty("default_wid")]
        public int DefaultWorkspaceId { get; set; }

        public string Email { get; set; }

        public string Fullname { get; set; }

        //BACKEND Y U STEP ON SNEK??
        [JsonProperty("timeofday_format")]
        public string TimeOfDayFormat { get; set; }

        public string DateFormat { get; set; }

        public bool StoreStartAndStopTime { get; set; }

        //TODO: Map to an Enum?
        public int BeginningOfWeek { get; set; }

        public string Language { get; set; }

        //TODO: Is this even needed
        public string ImageUrl { get; set; }

        //TODO: ?
        public bool SidebarPiechart { get; set; }

        public string At { get; set; } 
            
        //TODO: ?
        public int Retention { get; set; }
        
        //TODO: ?
        public bool RecordTimeline { get; set; }
        
        //TODO: ?
        public bool RenderTimeline { get; set; }
        
        //TODO: ?
        public bool TimelineEnabled { get; set; }
        
        //TODO: ?
        public bool TimelineExperiment { get; set; }
    }
}

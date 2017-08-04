using System;

namespace Toggl.Multivac.Models
{
    public interface IUser : IBaseModel
    {
        string ApiToken { get; }

        long DefaultWorkspaceId { get; }

        string Email { get; }

        string Fullname { get; }

        string TimeOfDayFormat { get; }

        string DateFormat { get; }

        bool StoreStartAndStopTime { get; }

        BeginningOfWeek BeginningOfWeek { get; }

        string Language { get; }

        //TODO: Is this even needed
        string ImageUrl { get; }

        //TODO: ?
        bool SidebarPiechart { get; }

        DateTimeOffset At { get; }

        //TODO: ?
        int Retention { get; }

        //TODO: ?
        bool RecordTimeline { get; }

        //TODO: ?
        bool RenderTimeline { get; }

        //TODO: ?
        bool TimelineEnabled { get; }

        //TODO: ?
        bool TimelineExperiment { get; }
    }
}

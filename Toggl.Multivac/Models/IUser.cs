namespace Toggl.Multivac.Models
{
    public interface IUser : IBaseModel
    {
        string ApiToken { get; }

        int DefaultWorkspaceId { get; }

        string Email { get; }

        string Fullname { get; }

        string TimeOfDayFormat { get; }

        string DateFormat { get; }

        bool StoreStartAndStopTime { get; }

        //TODO: Map to an Enum?
        int BeginningOfWeek { get; }

        string Language { get; }

        //TODO: Is this even needed
        string ImageUrl { get; }

        //TODO: ?
        bool SidebarPiechart { get; }

        string At { get; }

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

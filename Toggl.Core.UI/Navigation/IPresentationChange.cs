using System;

namespace Toggl.Core.UI.Navigation
{
    public interface IPresentationChange
    {
    }

    public class ShowReportsPresentationChange : IPresentationChange
    {
        public long? WorkspaceId { get; }

        public DateTimeOffset? StartDate { get; }

        public DateTimeOffset? EndDate { get; }
        public ShowReportsPresentationChange(long? workspaceId, DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            WorkspaceId = workspaceId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }

    public class ShowCalendarPresentationChange : IPresentationChange
    {
    }
}

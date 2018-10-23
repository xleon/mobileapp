namespace Toggl.Foundation.Diagnostics
{
    public enum MeasuredOperation
    {
        None,
        OpenStartView,
        OpenSelectTagsView,
        OpenSettingsView,
        EditTimeEntryFromCalendar,
        EditTimeEntryFromMainLog,
        OpenReportsViewForTheFirstTime,
        OpenCreateProjectViewFromStartTimeEntryView,
        OpenSelectProjectFromEditView,
        OpenReportsFromGiskard,
        CreateMainLogItemViewHolder,
        CreateMainLogSectionViewHolder,
        BindMainLogItemVH,
        BindMainLogSectionVH,
        BindMainLogSuggestionsVH,
        CreateMainLogSuggestionsViewHolder,
        MainActivityOnCreate
    }
}

namespace Toggl.Core.UI.ViewModels.Hints
{
    public sealed class ToggleReportsCalendarVisibilityHint
    {
        public bool ForceHide { get; }

        public ToggleReportsCalendarVisibilityHint(bool forceHide = false)
        {
            ForceHide = forceHide;
        }
    }
}

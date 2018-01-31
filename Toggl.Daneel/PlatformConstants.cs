using Toggl.Foundation;

namespace Toggl.Daneel
{
    public sealed class PlatformConstants : IPlatformConstants
    {
        public string HelpUrl { get; } = "https://support.toggl.com/toggl-timer-for-ios/";
        public string FeedbackEmailSubject { get; } = "Toggl iOS feedback";
    }
}

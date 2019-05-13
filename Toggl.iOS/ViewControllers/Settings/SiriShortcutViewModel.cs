using Intents;
using Toggl.Core.Models.Interfaces;
using Toggl.iOS.Models;

namespace Toggl.iOS.ViewControllers.Settings
{
    public class SiriShortcutViewModel
    {
        public string Title { get; }
        public string InvocationPhrase { get; }
        public string Description { get; }
        public string WorkspaceName { get; }
        public string ProjectName { get; }
        public string ClientName { get; }
        public string ProjectColor { get; }
        public bool HasTags { get; }
        public bool IsBillable { get; }
        public bool IsCustomStart { get; }
        public bool IsActive { get; }
        public SiriShortcutType Type { get; }
        public INVoiceShortcut VoiceShortcut { get; }

        public SiriShortcutViewModel(SiriShortcut siriShortcut, IThreadSafeProject project = null)
        {
            if (siriShortcut.VoiceShortcut != null)
            {
                VoiceShortcut = siriShortcut.VoiceShortcut;
                Type = VoiceShortcut.Shortcut.Intent.ShortcutType();
                Title = Type.Title();
                InvocationPhrase = VoiceShortcut.InvocationPhrase;
                IsActive = true;

                Description = (string)siriShortcut.Parameters[SiriShortcutParametersKey.Description];
                WorkspaceName = (string)siriShortcut.Parameters[SiriShortcutParametersKey.WorkspaceName];
                ProjectName = project?.Name;
                ClientName = project?.Client.Name;
                ProjectColor = project?.Color;
                HasTags = siriShortcut.Parameters[SiriShortcutParametersKey.Tags] != null;
                IsBillable = (bool)siriShortcut.Parameters[SiriShortcutParametersKey.Billable];

                if (Type == SiriShortcutType.CustomStart)
                {
                    Title = Description == null ? Type.Title() : $"Start timer: { Description }";
                    IsCustomStart = true;
                }
            }
            else
            {
                Type = siriShortcut.Type;
                Title = Type.Title();
                IsActive = false;
            }
        }

        public bool IsTimerShortcut()
        {
            return Type == SiriShortcutType.Stop || Type == SiriShortcutType.Start ||
                   Type == SiriShortcutType.Continue || Type == SiriShortcutType.CustomStart ||
                   Type == SiriShortcutType.StartFromClipboard;
        }

        public bool IsReportsShortcut()
        {
            return Type == SiriShortcutType.ShowReport || Type == SiriShortcutType.CustomReport;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Intents;
using Toggl.Core.Models;
using Toggl.iOS.Extensions;
using Toggl.iOS.Intents;
using Toggl.iOS.Services;

namespace Toggl.iOS.Models
{
    public struct SiriShortcutParametersKey
    {
        public static string Description = "Description";
        public static string WorkspaceId = "WorkspaceId";
        public static string WorkspaceName = "WorkspaceName";
        public static string Billable = "Billable";
        public static string Tags = "Tags";
        public static string ProjectId = "ProjectId";
        public static string ReportPeriod = "ReportPeriod";
    }

    public class SiriShortcut
    {
        public SiriShortcutType Type { get; }
        public string Identifier { get; }
        public Dictionary<string, object> Parameters { get; }
        public INVoiceShortcut VoiceShortcut { get; }

        private INIntent Intent { get; }

        public SiriShortcut(INVoiceShortcut voiceShortcut)
        {
            VoiceShortcut = voiceShortcut;
            Identifier = voiceShortcut.Identifier.AsString();
            Intent = voiceShortcut.Shortcut.Intent;
            Type = Intent.ShortcutType();

            if (Intent is ShowReportPeriodIntent showReportPeriodIntent)
            {
                Parameters = new Dictionary<string, object> {
                    { SiriShortcutParametersKey.ReportPeriod, showReportPeriodIntent.Period.ToReportPeriod() }
                };
            }

            if (Intent is StartTimerIntent startTimerIntent)
            {
                Parameters = new Dictionary<string, object> {
                    { SiriShortcutParametersKey.Description, startTimerIntent.EntryDescription },
                    { SiriShortcutParametersKey.WorkspaceId, startTimerIntent.Workspace?.Identifier },
                    { SiriShortcutParametersKey.WorkspaceName, startTimerIntent.Workspace?.DisplayString },
                    { SiriShortcutParametersKey.Billable, startTimerIntent.Billable?.Identifier == "True" },
                    { SiriShortcutParametersKey.Tags, startTimerIntent.Tags == null ? null : stringToLongCollection(startTimerIntent.Tags.Select(tag => tag.Identifier)) },
                    { SiriShortcutParametersKey.ProjectId, startTimerIntent.ProjectId?.Identifier }
                };
            }

            if (Intent is StartTimerFromClipboardIntent startTimerFromClipboardIntent)
            {
                Parameters = new Dictionary<string, object> {
                    { SiriShortcutParametersKey.WorkspaceId, startTimerFromClipboardIntent.Workspace?.Identifier },
                    { SiriShortcutParametersKey.WorkspaceName, startTimerFromClipboardIntent.Workspace?.DisplayString },
                    { SiriShortcutParametersKey.Billable, startTimerFromClipboardIntent.Billable?.Identifier == "True" },
                    { SiriShortcutParametersKey.Tags, startTimerFromClipboardIntent.Tags == null ? new long[0] : stringToLongCollection(startTimerFromClipboardIntent.Tags.Select(tag => tag.Identifier)) },
                    { SiriShortcutParametersKey.ProjectId, stringToLong(startTimerFromClipboardIntent.ProjectId?.Identifier) }
                };
            }
        }

        public SiriShortcut(SiriShortcutType type)
        {
            Type = type;
        }

        public static SiriShortcut[] DefaultShortcuts = new[]
        {
            new SiriShortcut(
                SiriShortcutType.Start
            ),
            new SiriShortcut(
                SiriShortcutType.Stop
            ),
            new SiriShortcut(
                SiriShortcutType.Continue
            ),
            new SiriShortcut(
                SiriShortcutType.StartFromClipboard
            ),
            new SiriShortcut(
                SiriShortcutType.CustomStart
            ),
            new SiriShortcut(
                SiriShortcutType.ShowReport
            ),
            new SiriShortcut(
                SiriShortcutType.CustomReport
            )
        };

        private long? stringToLong(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (long)Convert.ToDouble(str);
        }

        private IEnumerable<long> stringToLongCollection(IEnumerable<string> strings)
        {
            if (strings.Count() == 0)
                return new long[0];

            return strings.Select(stringToLong).Cast<long>();
        }
    }
}

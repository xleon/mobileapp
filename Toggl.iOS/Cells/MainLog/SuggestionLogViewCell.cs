using System;
using System.Diagnostics;
using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.iOS.Cells;
using Toggl.iOS.Shared.Extensions;
using Toggl.iOS.Transformations;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Views
{
    public sealed partial class SuggestionLogViewCell : BaseTableViewCell<SuggestionLogItemViewModel>
    {
        public static readonly string Identifier = "SuggestionCell";
        public static readonly UINib Nib;

        ProjectTaskClientToAttributedString projectTaskClientToAttributedString;

        static SuggestionLogViewCell()
        {
            Nib = UINib.FromName(nameof(SuggestionLogViewCell), NSBundle.MainBundle);
        }

        protected SuggestionLogViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            IsAccessibilityElement = true;
            AccessibilityHint = Resources.SuggestionAccessibilityHint;
            AccessibilityTraits = UIAccessibilityTrait.Button;
            NoDescriptionLabel.Text = Resources.NoDescription;

            DescriptionFadeView.FadeRight = true;
            ProjectFadeView.FadeRight = true;

            projectTaskClientToAttributedString = new ProjectTaskClientToAttributedString(
                ProjectLabel.Font.CapHeight,
                Colors.TimeEntriesLog.ClientColor.ToNativeColor()
            );

            ArrowImage.SetTemplateColor(ColorAssets.Text4);
        }

        protected override void UpdateView()
        {
            if (Item.Suggestion == null) return;

            updateAccessibilityProperties();

            DescriptionLabel.Text = Item.Suggestion.Description;
            DescriptionLabel.Hidden = Item.Suggestion.Description == string.Empty;
            NoDescriptionLabel.Hidden = Item.Suggestion.Description != string.Empty;

            prefixWithProviderNameInDebug();

            var hasProject = Item.Suggestion.ProjectId != null;
            ProjectFadeView.Hidden = !hasProject;

            ProjectLabel.AttributedText = projectTaskClientToAttributedString.Convert(Item.Suggestion);
        }

        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();
            ClipsToBounds = false;
            CardView.UpdateCardView();
        }

        private void updateAccessibilityProperties()
        {
            AccessibilityLabel = $"{Resources.Suggestion}, ";
            if (!string.IsNullOrEmpty(Item.Suggestion.Description))
                AccessibilityLabel += $"{Item.Suggestion.Description}, ";
            if (Item.Suggestion.HasProject)
                AccessibilityLabel += $"{Resources.Project}: {Item.Suggestion.ProjectName }, ";
            if (Item.Suggestion.HasTask)
                AccessibilityLabel += $"{Resources.Task}: {Item.Suggestion.TaskName}, ";
            if (Item.Suggestion.HasClient)
                AccessibilityLabel += $"{Resources.Client}: {Item.Suggestion.ClientName}";
        }

        [Conditional("DEBUG")]
        private void prefixWithProviderNameInDebug()
        {
            var prefix = Item.Suggestion.ProviderType.ToString().Substring(0, 4);
            DescriptionLabel.Text = $"{prefix} {Item.Suggestion.Description}";
        }
    }
}

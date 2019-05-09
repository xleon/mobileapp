using System;

using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.iOS.Cells;
using Toggl.iOS.Extensions;
using Toggl.iOS.Models;
using UIKit;

namespace Toggl.iOS.Views.Settings
{
    public partial class CustomSiriShortcutCell : BaseTableViewCell<SiriShortcut>
    {
        public static readonly string Identifier = nameof(CustomSiriShortcutCell);
        public static readonly UINib Nib;

        static CustomSiriShortcutCell()
        {
            Nib = UINib.FromName("CustomSiriShortcutCell", NSBundle.MainBundle);
        }

        protected CustomSiriShortcutCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        protected override void UpdateView()
        {
            TitleLabel.Text = Item.Title;
            DetailsLabel.Text = "Entry details";
            InvocationLabel.Text = $"\"{Item.InvocationPhrase}\"";
            InvocationLabel.TextColor = Colors.Siri.InvocationPhrase.ToNativeColor();
        }
    }
}


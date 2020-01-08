using Toggl.Core.UI.Helper;
using Toggl.iOS.Extensions;
using UIKit;

namespace Toggl.iOS.Views.Reports
{
    internal sealed class BarLegendLabel : UILabel
    {
        public BarLegendLabel(string dayInitial, string shortDate) : this($"{dayInitial}\n{shortDate}")
        {
        }

        public BarLegendLabel(string text)
        {
            Text = text;
            TextAlignment = UITextAlignment.Center;
            Lines = 2;
            TextColor = ColorAssets.Text3;
            Font = UIFont.SystemFontOfSize(12);
            AdjustsFontSizeToFitWidth = true;
        }
    }
}

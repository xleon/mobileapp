using Foundation;
using Toggl.Daneel.Extensions;
using Toggl.Shared;
using UIKit;
using Colors = Toggl.Core.UI.Helper.Colors;

namespace Toggl.Daneel.Views.EditDuration
{
    public static class DurationFieldTextFormatter
    {
        private static readonly UIColor placeHolderColor = Colors.Common.PlaceholderText.ToNativeColor();

        public static NSAttributedString AttributedStringFor(string durationText, UIFont font)
        {
            durationText = durationText ?? "";
            var prefixLength = DurationHelper.LengthOfDurationPrefix(durationText);
            var result = new NSMutableAttributedString(durationText, font: font.GetMonospacedDigitFont(), foregroundColor: UIColor.Black);
            result.AddAttribute(UIStringAttributeKey.ForegroundColor, placeHolderColor, new NSRange(0, prefixLength));
            return result;
        }
    }
}

using System;
using System.Globalization;
using Foundation;
using MvvmCross.Platform.Converters;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Converters
{
    public sealed class ReportPercentageLabelValueConverter : MvxValueConverter<float?, NSAttributedString>
    {
        private const int fontSize = 24;

        private static readonly UIColor normalColor = Color.Reports.PercentageActivated.ToNativeColor();
        private static readonly UIColor disabledColor = Color.Reports.Disabled.ToNativeColor();

        private readonly UIStringAttributes normalAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = normalColor
        };

        private readonly UIStringAttributes disabledAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = disabledColor
        };

        protected override NSAttributedString Convert(float? value, Type targetType, object parameter, CultureInfo culture)
        {
            var isEmpty = value == null;
            var actualValue = isEmpty ? 0 : value.Value;

            var percentage = $"{actualValue.ToString("0.00")}%";

            var attributes = isEmpty ? disabledAttributes : normalAttributes;
            return new NSAttributedString(percentage, attributes);
        }
    }
}

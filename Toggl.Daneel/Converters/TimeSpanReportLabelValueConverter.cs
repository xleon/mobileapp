using System;
using System.Globalization;
using Foundation;
using MvvmCross.Platform.Converters;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Converters
{
    public sealed class TimeSpanReportLabelValueConverter : MvxValueConverter<TimeSpan, NSAttributedString>
    {
        private const int fontSize = 24;

        private static readonly UIColor normalColor = Color.Reports.TotalTimeActivated.ToNativeColor();
        private static readonly UIColor disabledColor = Color.Reports.Disabled.ToNativeColor();

        private static readonly TimeSpanToDurationValueConverter innerConverter = new TimeSpanToDurationValueConverter();

        private readonly UIStringAttributes normalAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = normalColor
        };

        private readonly UIStringAttributes normalSecondsAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Light),
            ForegroundColor = normalColor
        };

        private readonly UIStringAttributes disabledAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Medium),
            ForegroundColor = disabledColor
        };

        private readonly UIStringAttributes disabledSecondsAttributes = new UIStringAttributes
        {
            Font = UIFont.SystemFontOfSize(fontSize, UIFontWeight.Light),
            ForegroundColor = disabledColor
        };

        protected override NSAttributedString Convert(TimeSpan value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeString = innerConverter.Convert(value, targetType, parameter, culture) as string;
            var lengthOfHours = value.Hours.ToString().Length;

            var isEmpty = value.Ticks == 0;
            var result = new NSMutableAttributedString(timeString);

            var attributes = isEmpty ? disabledAttributes : normalAttributes;
            var secondsAttributes = isEmpty ? disabledSecondsAttributes : normalSecondsAttributes;

            result.AddAttributes(attributes, new NSRange(0, lengthOfHours + 3));
            result.AddAttributes(secondsAttributes, new NSRange(lengthOfHours + 3, 3));

            return result;
        }
    }
}

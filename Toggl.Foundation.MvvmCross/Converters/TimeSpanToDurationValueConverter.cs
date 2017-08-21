using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public class TimeSpanToDurationValueConverter : MvxValueConverter<TimeSpan, string>
    {
        protected override string Convert(TimeSpan value, Type targetType, object parameter, CultureInfo culture)
            => $"{(int)value.TotalHours}:{value.Minutes.ToString("D2")}:{value.Seconds.ToString("D2")}";
    }
}

using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class DateTimeToTimeConverter : MvxValueConverter<DateTimeOffset, string>
    {
        protected override string Convert(DateTimeOffset value, Type targetType, object parameter, CultureInfo culture)
            => TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local).ToString("HH:mm");
    }
}

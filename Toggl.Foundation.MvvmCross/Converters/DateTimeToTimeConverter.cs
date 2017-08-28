using System;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class DateTimeToTimeConverter : MvxValueConverter<DateTimeOffset, string>
    {
        protected override string Convert(DateTimeOffset value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value.ToString("t");
    }
}

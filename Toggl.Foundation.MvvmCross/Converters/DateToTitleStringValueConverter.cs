using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public class DateToTitleStringValueConverter : MvxValueConverter<DateTimeOffset, string>
    {
        protected override string Convert(DateTimeOffset value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.Date == DateTime.UtcNow.Date)
                return Resources.Today;
            
            if (value.Date.AddDays(1) == DateTime.UtcNow.Date)
                return Resources.Yesterday;

            return $"{value:ddd, dd MMM}";
        }
    }
}

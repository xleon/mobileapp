using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public class SecondsToDurationValueConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var timeSpan = TimeSpan.FromSeconds(value);
            return $"{ (int)timeSpan.TotalHours }:{ timeSpan.Minutes.ToString("D2") }:{ timeSpan.Seconds.ToString("D2") }";
        }
    }
}

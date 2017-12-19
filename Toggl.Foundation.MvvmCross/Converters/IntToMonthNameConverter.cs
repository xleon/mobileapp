using System;
using System.Globalization;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class IntToMonthNameConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value < 1 || value > 12)
                throw new ArgumentOutOfRangeException($"{nameof(value)} must be in range [1 - 12]");

            return culture.DateTimeFormat.GetMonthName(value);
        }
    }
}

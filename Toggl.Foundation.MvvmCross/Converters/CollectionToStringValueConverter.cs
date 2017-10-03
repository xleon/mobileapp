using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MvvmCross.Platform.Converters;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public sealed class CollectionToStringValueConverter<T> : MvxValueConverter<IEnumerable<T>, string>
    {
        protected override string Convert(IEnumerable<T> value, Type targetType, object parameter, CultureInfo culture)
            => value.Aggregate(
                   new StringBuilder(),
                   (builder, str) => builder.Append($" {str.ToString()}"))
               .Remove(0, 1)
               .ToString();
    }
}

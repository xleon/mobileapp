using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Binding.Bindings.SourceSteps;
using MvvmCross.Binding.Combiners;
using MvvmCross.Platform.Converters;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Combiners
{
    [Preserve(AllMembers = true)]
    public sealed class DateTimeOffsetDateFormatValueCombiner : MvxValueCombiner
    {
        private readonly TimeZoneInfo timeZone;

        private readonly bool useLongFormat;

        public DateTimeOffsetDateFormatValueCombiner(TimeZoneInfo timeZone, bool useLongFormat = true)
        {
            this.timeZone = timeZone;
            this.useLongFormat = useLongFormat;
        }

        public override bool TryGetValue(IEnumerable<IMvxSourceStep> steps, out object value)
        {
            DateTimeOffset date;
            DateFormat format;

            try
            {
                (date, format) = getParameters(steps);
            }
            catch (ArgumentException)
            {
                value = MvxBindingConstant.UnsetValue;
                return false;
            }

            var formatVariant = useLongFormat ? format.Long : format.Short;
            value = getDateTimeOffsetInCorrectTimeZone(date).ToString(formatVariant);
            return true;
        }

        private (DateTimeOffset, DateFormat) getParameters(IEnumerable<IMvxSourceStep> steps)
        {
            var parameters = steps.ToArray();

            if (parameters.Length != 2)
                throw new ArgumentException(
                    $"The {nameof(DateTimeOffsetDateFormatValueCombiner)} must be used with 2 parameters: a DateTimeOffset and a DateFormat. It was instead used with {parameters.Length} parameters.");

            var date = parameters[0].GetValue() as DateTimeOffset?;
            if (!date.HasValue)
                throw new ArgumentException($"The first parameter of the {nameof(DateTimeOffsetDateFormatValueCombiner)} must be a {nameof(DateTimeOffset)}, but the given parameter is of type {parameters[0].SourceType.FullName}");

            var format = parameters[1].GetValue() as DateFormat?;
            if (!format.HasValue)
                throw new ArgumentException($"The second parameter of the {nameof(DateTimeOffsetDateFormatValueCombiner)} must be a {nameof(DateFormat)}, but the given parameter is of type {parameters[1].SourceType.FullName}");

            return (date.Value, format.Value);
        }

        private DateTimeOffset getDateTimeOffsetInCorrectTimeZone(DateTimeOffset value)
            => value == default(DateTimeOffset) ? value : TimeZoneInfo.ConvertTime(value, timeZone);
    }
}

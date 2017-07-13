using System;
using System.Globalization;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Toggl.Foundation.MvvmCross.Converters;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.Converters
{
    public class DateToTitleStringValueConverterTests
    {
        public class TheConvertMethod
        {
            [Fact]
            public void ReturnsASpecialCaseStringForTheCurrentDay()
            {
                var converter = new DateToTitleStringValueConverter();
                var date = DateTime.UtcNow;

                var result = converter.Convert(date, typeof(DateTime), null, CultureInfo.CurrentCulture);

                result.Should().Be(Resources.Today);
            }

            [Fact]
            public void ReturnsASpecialCaseStringForThePreviousDay()
            {
                var converter = new DateToTitleStringValueConverter();
                var date = DateTime.UtcNow.AddDays(-1);

                var result = converter.Convert(date, typeof(DateTime), null, CultureInfo.CurrentCulture);

                result.Should().Be(Resources.Yesterday);
            }

            [Property]
            public Property ReturnsAFormattedStringForAnyOtherDate()
            {
                var arb = Arb.Default.DateTime().Filter(d => d < DateTime.UtcNow.AddDays(-1));

                return Prop.ForAll(arb, date =>
                {
                    var converter = new DateToTitleStringValueConverter();

                    var result = converter.Convert(date, typeof(DateTime), null, CultureInfo.CurrentCulture);

                    result.Should().Be($"{date:ddd, dd MMM}");
                });
            }
        }
    }
}

using System.Globalization;
using FluentAssertions;
using Toggl.Foundation.MvvmCross.Converters;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.Converters
{
    public class SecondsToDurationValueConverterTests
    {
        public class TheConvertMethod
        {
            [Fact]
            public void WorksWithZero()
            {
                var converter = new SecondsToDurationValueConverter();

                var result = converter.Convert(0, typeof(string), null, CultureInfo.CurrentCulture);

                result.Should().Be("0:00:00");
            }

            [Fact]
            public void WorksWithMoreThan24Hours()
            {
                var converter = new SecondsToDurationValueConverter();
                var seconds = 90000; //25 hours

                var result = converter.Convert(seconds, typeof(string), null, CultureInfo.CurrentCulture);

                result.Should().Be("25:00:00");
            }

            [Fact]
            public void WorksForNormalCase()
            {
                var converter = new SecondsToDurationValueConverter();
                var seconds = 5 * 60 * 60 + 4 * 60 + 3;

                var result = converter.Convert(seconds, typeof(string), null, CultureInfo.CurrentCulture);

                result.Should().Be("5:04:03");
            }
        }
    }
}

using System;
using System.Globalization;
using FluentAssertions;
using MvvmCross;
using MvvmCross.Converters;
using MvvmCross.IoC;
using MvvmCross.Logging;
using MvvmCross.Tests;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Multivac;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.Converters
{
    public sealed class ParametricTimeSpanToDurationValueConverterTests
    {
        public static ParametricTimeSpanToDurationValueConverter converter = new ParametricTimeSpanToDurationValueConverter();

        public sealed class TheConvertMethod
        {
            static TheConvertMethod()
            {
                var log = Substitute.For<IMvxLog>();
                var logProvider = Substitute.For<IMvxLogProvider>();

                var ioc = MvxIoCProvider.Initialize();
                ioc.RegisterSingleton(ioc);
                ioc.RegisterSingleton(logProvider);

                logProvider.GetLogFor(Arg.Any<Type>()).Returns(log);
            }

            [Theory, LogIfTooSlow]
            [InlineData(null)]
            [InlineData((int)DurationFormat.Classic)]
            [InlineData("Classic")]
            [InlineData(false)]
            public void FailsForFormatsWhichAreNotDurationFormat(object parameter)
            {
                var convertedValue = converter.Convert(TimeSpan.Zero, typeof(string), parameter, CultureInfo.CurrentCulture);

                convertedValue.Should().Be(MvxBindingConstant.UnsetValue);
            }

            [Theory, LogIfTooSlow]
            [InlineData(3)]
            [InlineData(4)]
            [InlineData(5)]
            [InlineData(10)]
            [InlineData(100)]
            public void FailsForDurationFormatsWhichAreOutOfRange(int parameter)
            {
                var convertedValue = converter.Convert(TimeSpan.Zero, typeof(string), (DurationFormat)parameter,
                    CultureInfo.CurrentCulture);

                convertedValue.Should().Be(MvxBindingConstant.UnsetValue);
            }

            [Theory, LogIfTooSlow]
            [InlineData(DurationFormat.Classic)]
            [InlineData(DurationFormat.Decimal)]
            [InlineData(DurationFormat.Improved)]
            public void ReturnsNonEmptyStringForValidFormats(DurationFormat format)
            {
                var convertedValue = converter.Convert(TimeSpan.Zero, typeof(string), format, CultureInfo.CurrentCulture);

                convertedValue.Should().BeOfType<string>();
                ((string)convertedValue).Length.Should().BeGreaterThan(0);
            }
        }
    }
}

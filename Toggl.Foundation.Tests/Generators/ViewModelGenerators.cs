using System;
using System.Linq;
using System.Reactive.Linq;
using FsCheck;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Extensions;
using MonthPredicate = System.Func<int, bool>;
using TimeEntry = Toggl.Foundation.Models.TimeEntry;

namespace Toggl.Foundation.Tests.Generators
{
    public static class ViewModelGenerators
    {
        public static Gen<TimeEntriesLogViewModel> ForTimeEntriesLogViewModel(MonthPredicate filter)
        {
            var monthsGenerator = Gen.Choose(1, 12).Where(filter);
            var yearGenerator = Gen.Choose(2007, DateTime.UtcNow.Year);

            return Arb.Default
                .Array<DateTimeOffset>()
                .Generator
                .Select(dateTimes =>
                {
                    var source = Substitute.For<ITogglDataSource>();
                    var timeService = Substitute.For<ITimeService>();
                    var viewModel = new TimeEntriesLogViewModel(source);

                    var year = yearGenerator.Sample(0, 1).First();

                    var observable = dateTimes
                        .Select(newDateWithGenerator(monthsGenerator, year))
                        .Select(d => TimeEntry.Builder.Create(-1).SetStart(d).SetDescription("").Build())
                        .Apply(Observable.Return);

                    source.TimeEntries.GetAll().Returns(observable);

                    return viewModel;
                });
        }

        private static Func<DateTimeOffset, DateTimeOffset> newDateWithGenerator(Gen<int> monthGenerator, int year)
        {
            var month = monthGenerator.Sample(0, 1).First();
            var day = Gen.Choose(1, DateTime.DaysInMonth(year, month)).Sample(0, 1).First();

            return dateTime =>
                new DateTime(year, month, day, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
    }
}

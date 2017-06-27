using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Models;
using Xunit;
using TimeEntry = Toggl.Ultrawave.Models.TimeEntry;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class TimeEntriesViewModelTests
    {
        public abstract class TimeEntriesViewModelTest : BaseViewModelTests<TimeEntriesViewModel>
        {
            protected override TimeEntriesViewModel CreateViewModel()
                => new TimeEntriesViewModel(DataSource);
        }

        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentsIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntriesViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheTimeEntriesProperty : TimeEntriesViewModelTest
        {
            [Fact]
            public async Task ReturnsAllTimeEntries()
            {
                var timeEntries = new List<ITimeEntry>()
                {
                    new TimeEntry { Description = "entry1" },
                    new TimeEntry { Description = "entry2" },
                    new TimeEntry { Description = "entry3" }
                };
                var observable = Observable.Return(timeEntries);
                DataSource.TimeEntries.GetAll().Returns(observable);

                await ViewModel.Initialize();

                ViewModel.TimeEntries.Should().HaveCount(timeEntries.Count);
                ViewModel.TimeEntries.ShouldBeEquivalentTo(timeEntries);
            }
        }
    }
}

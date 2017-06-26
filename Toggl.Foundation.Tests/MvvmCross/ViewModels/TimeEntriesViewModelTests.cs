using System.Collections.Generic;
using System.Reactive.Disposables;
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
        public class TheTimeEntriesProperty : BaseViewModelTests<TimeEntriesViewModel>
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

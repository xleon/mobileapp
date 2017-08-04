using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.DataSources
{
    public class UserDataSourceTests
    {
        public class UserDataSourceTest
        {
            protected ITimeEntriesSource TimeEntriesSource { get; }

            protected string ValidDescription { get; } = "Testing software";

            protected DateTimeOffset ValidTime { get; } = DateTimeOffset.Now;

            protected IRepository<IDatabaseTimeEntry> Repository { get; } = Substitute.For<IRepository<IDatabaseTimeEntry>>();

            public UserDataSourceTest()
            {
                TimeEntriesSource = new TimeEntriesDataSource(Repository);
                Repository.Create(Arg.Any<IDatabaseTimeEntry>())
                          .Returns(info => Observable.Return(info.Arg<IDatabaseTimeEntry>()));
            }
        }

        public class TheStartMethod : UserDataSourceTest
        {
            [Fact]
            public async Task CreatesANewTimeEntryInTheDatabase()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Any<IDatabaseTimeEntry>());
            }

            [Fact]
            public async Task CreatesADirtyTimeEntry()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.IsDirty));
            }

            [Theory]
            [InlineData(true)]
            [InlineData(false)]
            public async Task CreatesATimeEntryWithTheProvidedValueForBillable(bool billable)
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, billable);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Billable == billable));
            }

            [Fact]
            public async Task CreatesATimeEntryWithTheProvidedValueForDescription()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);
                
                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Description == ValidDescription));
            }

            [Fact]
            public async Task CreatesATimeEntryWithTheProvidedValueForStartTime()
            {
                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Start == ValidTime));
            }

            [Fact]
            public async Task SetstheCreatedTimeEntryAsTheCurrentlyRunningTimeEntry()
            {
                var observer = new TestScheduler().CreateObserver<ITimeEntry>();
                TimeEntriesSource.CurrentlyRunningTimeEntry.Where(te => te != null).Subscribe(observer);

                await TimeEntriesSource.Start(ValidTime, ValidDescription, true);

                var currentlyRunningTimeEntry = observer.Messages.Single().Value.Value;
                await Repository.Received().Create(Arg.Is<IDatabaseTimeEntry>(te => te.Start == currentlyRunningTimeEntry.Start));
            }
        }
    }
}

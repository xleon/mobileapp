using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Xunit;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;

namespace Toggl.Foundation.Tests.DataSources
{
    public sealed class TogglDataSourceTests
    {
        public abstract class TogglDataSourceTest
        {
            protected ITogglDataSource DataSource { get; }
            protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
            protected IScheduler Scheduler { get; } = new TestScheduler();

            public TogglDataSourceTest()
            {
                DataSource = new TogglDataSource(Database, Api, Scheduler);
            }
        }

        public sealed class TheLogoutMethod : TogglDataSourceTest
        {
            [Fact]
            public void ClearsTheDatabase()
            {
                DataSource.Logout();

                Database.Received(1).Clear();
            }
        }
    }
}

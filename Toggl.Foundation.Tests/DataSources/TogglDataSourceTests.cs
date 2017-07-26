using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;
using Xunit;

namespace Toggl.Foundation.Tests.DataSources
{
    public class TogglDataSourceTests
    {
        public class TogglDataSourceTest
        {
            protected ITogglDataSource DataSource { get; }
            protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();

            public TogglDataSourceTest()
            {
                DataSource = new TogglDataSource(Database, Api);
            }
        }

        public class TheLogoutMethod : TogglDataSourceTest
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

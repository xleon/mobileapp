using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Tests.Mocks;
using Toggl.Shared;
using Xunit;

namespace Toggl.Core.Tests.Interactors
{
    public class GetPreferencesInteractorTests
    {
        public sealed class GetPreferencesInteractor : BaseInteractorTests
        {
            [Fact]
            public async Task CorrectPreferencesAreRetrieved()
            {
                IThreadSafePreferences prefs = Substitute.For<IThreadSafePreferences>();
                DataSource.Preferences.Current.Returns(Observable.Return(prefs));
             
                var result = await InteractorFactory.GetPreferences().Execute().FirstAsync();

                result.Should().Be(prefs);
            }
        }
    }
}

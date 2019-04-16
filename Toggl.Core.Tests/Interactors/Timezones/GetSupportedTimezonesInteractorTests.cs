using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Core.Interactors.Timezones;
using Toggl.Core.Serialization;
using Xunit;

namespace Toggl.Core.Tests.Interactors.Timezones
{
    public sealed class GetSupportedTimezonesInteractorTests
    {
        public sealed class TheExecuteMethod
        {
            [Fact, LogIfTooSlow]
            public async Task ReturnsTheTimezonesApiResponse()
            {
                var interactor = new GetSupportedTimezonesInteractor(new JsonSerializer());

                var returnedTimezones = await interactor.Execute();

                returnedTimezones.Should().NotBeEmpty();
            }
        }
    }
}

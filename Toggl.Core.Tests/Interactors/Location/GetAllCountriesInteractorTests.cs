using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.Core.Interactors;
using Xunit;
using Toggl.Shared.Models;

namespace Toggl.Core.Tests.Interactors
{
    public sealed class GetAllCountriesInteractorTests
    {
        public sealed class TheExecuteMethod : BaseInteractorTests
        {
            private readonly GetAllCountriesInteractor interactor;

            public TheExecuteMethod()
            {
                interactor = new GetAllCountriesInteractor();
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotReturnAnEmptyList()
            {
                var countries = await interactor.Execute();

                countries.Should().NotBeEmpty();
            }
        }
    }
}

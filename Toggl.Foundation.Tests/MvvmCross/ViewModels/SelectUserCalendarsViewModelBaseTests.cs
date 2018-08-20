using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Interactors;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using Toggl.Multivac;
using Xunit;
using static Toggl.Multivac.Extensions.FunctionalExtensions;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class SelectUserCalendarsViewModelBaseTests
    {
        public sealed class MockSelectUserCalendarsViewModel : SelectUserCalendarsViewModelBase
        {
            public MockSelectUserCalendarsViewModel(IInteractorFactory interactorFactory)
                : base(interactorFactory)
            {
            }
        }

        public abstract class SelectUserCalendarsViewModelBaseTest : BaseViewModelTests<MockSelectUserCalendarsViewModel>
        {
            protected override MockSelectUserCalendarsViewModel CreateViewModel()
                => new MockSelectUserCalendarsViewModel(InteractorFactory);
        }

        public sealed class TheInitializeMethod : SelectUserCalendarsViewModelBaseTest
        {
            [Fact, LogIfTooSlow]
            public async Task FillsTheCalendarList()
            {
                var userCalendarsObservable = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false))
                    .Apply(Observable.Return);
                InteractorFactory.GetUserCalendars().Execute().Returns(userCalendarsObservable);

                await ViewModel.Initialize();

                ViewModel.Calendars.Should().HaveCount(3);
                ViewModel.Calendars.ForEach(group => group.Should().HaveCount(3));
            }
        }
    }
}

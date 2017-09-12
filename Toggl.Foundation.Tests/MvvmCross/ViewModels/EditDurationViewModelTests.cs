using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class EditDurationViewModelTests
    {
        public abstract class EditDurationViewModelTest : BaseViewModelTests<EditDurationViewModel>
        {
            protected override EditDurationViewModel CreateViewModel()
                => new EditDurationViewModel(NavigationService, TimeService);
        }

        public sealed class TheConstructor : EditDurationViewModelTest
        {
            [Theory]
            [ClassData(typeof(TwoParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useNavigationService, bool useTimeService)
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new EditDurationViewModel(navigationService, timeService);

                tryingToConstructWithEmptyParameters.ShouldThrow<ArgumentNullException>();
            }

        }

        public sealed class TheDurationProperty : EditDurationViewModelTest
        {
            [Fact]
            public async Task WhenChangedWhileUpdatingTheRunningTimeEntryTriggersTheUpdateOfTheStartTime()
            {
                var now = DateTimeOffset.Now;
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                TimeService.CurrentDateTime.Returns(now);
                await ViewModel.Initialize(parameter);
                
                ViewModel.Duration = TimeSpan.FromHours(4);

                var expectedStart = start.AddHours(-2);
                ViewModel.StartTime.Should().BeSameDateAs(expectedStart);
            }

            [Fact]
            public async Task WhenChangedWhileUpdatingFinishedTimeEntryTriggersTheUpdateOfTheStopTime()
            {
                var now = DateTimeOffset.Now;
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, now);
                TimeService.CurrentDateTime.Returns(now);
                await ViewModel.Initialize(parameter);

                ViewModel.Duration = TimeSpan.FromHours(4);

                var expectedStop = now.AddHours(2);
                ViewModel.StopTime.Should().BeSameDateAs(expectedStop);
            }

            [Fact]
            public async Task IsUpdatedAccordingToTimeServiceForRunningTimeEntries()
            {
                var now = DateTimeOffset.Now;
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                var tickSubject = new Subject<DateTimeOffset>();
                var tickObservable = tickSubject.AsObservable().Publish();
                tickObservable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(tickObservable);
                TimeService.CurrentDateTime.Returns(now);
                await ViewModel.Initialize(parameter);
                
                tickSubject.OnNext(now.AddHours(2));

                ViewModel.Duration.Hours.Should().Be(4);
            }
        }

        public sealed class TheStopTimeProperty : EditDurationViewModelTest
        {
            [Fact]
            public async Task IsUpdatedAccordingToTimeServiceForRunningTimeEntries()
            {
                var now = DateTimeOffset.Now;
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                var tickSubject = new Subject<DateTimeOffset>();
                var tickObservable = tickSubject.AsObservable().Publish();
                tickObservable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(tickObservable);
                TimeService.CurrentDateTime.Returns(now);
                await ViewModel.Initialize(parameter);

                var newCurrentTime = now.AddHours(2);
                tickSubject.OnNext(newCurrentTime);

                ViewModel.StopTime.Should().BeSameDateAs(newCurrentTime);
            }
        }

        public sealed class TheInitializeMethod : EditDurationViewModelTest
        {
            [Fact]
            public async Task SetsTheStartTime()
            {
                var start = DateTimeOffset.Now;
                var parameter = DurationParameter.WithStartAndStop(start, null);

                await ViewModel.Initialize(parameter);

                ViewModel.StartTime.Should().Be(start);
            }

            [Fact]
            public async Task SetsTheStartTimeToCurrentTimeIfParameterDoesNotHaveStartTime()
            {
                var start = DateTimeOffset.Now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                var now = DateTimeOffset.Now;
                TimeService.CurrentDateTime.Returns(now);

                await ViewModel.Initialize(parameter);

                ViewModel.StartTime.Should().BeSameDateAs(now);
            }


            [Fact]
            public async Task SetsTheStopTimeToParameterStopTimeIfParameterHasStopTime()
            {
                var start = DateTimeOffset.Now.AddHours(-4);
                var stop = start.AddHours(2);
                var parameter = DurationParameter.WithStartAndStop(start, stop);

                await ViewModel.Initialize(parameter);

                ViewModel.StartTime.Should().BeSameDateAs(stop);
            }

            [Fact]
            public async Task SubscribesToCurrentTimeObservableIfParameterDoesNotHaveStopTime()
            {
                var start = DateTimeOffset.Now;
                var parameter = DurationParameter.WithStartAndStop(start, null);
                
                await ViewModel.Initialize(parameter);

                TimeService.CurrentDateTimeObservable.Received().Subscribe(Arg.Any<AnonymousObserver<DateTimeOffset>>());
            }
        }
        
        public sealed class TheCloseCommand : EditDurationViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel));
            }
        }
    }
}

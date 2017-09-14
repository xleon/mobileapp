using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FluentAssertions;
using FsCheck.Xunit;
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
            [Property]
            public void WhenChangedWhileUpdatingTheRunningTimeEntryTriggersTheUpdateOfTheStartTime(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare(parameter);
                
                ViewModel.Duration = TimeSpan.FromHours(4);

                var expectedStart = start.AddHours(-2);
                ViewModel.StartTime.Should().BeSameDateAs(expectedStart);
            }

            [Property]
            public void WhenChangedWhileUpdatingFinishedTimeEntryTriggersTheUpdateOfTheStopTime(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, now);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare(parameter);
                
                ViewModel.Duration = TimeSpan.FromHours(4);

                var expectedStop = now.AddHours(2);
                ViewModel.StopTime.Should().BeSameDateAs(expectedStop);
            }

            [Property]
            public void IsUpdatedAccordingToTimeServiceForRunningTimeEntries(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                var tickSubject = new Subject<DateTimeOffset>();
                var tickObservable = tickSubject.AsObservable().Publish();
                tickObservable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(tickObservable);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare(parameter);

                tickSubject.OnNext(now.AddHours(2));

                ViewModel.Duration.Hours.Should().Be(4);
            }
        }

        public sealed class TheStopTimeProperty : EditDurationViewModelTest
        {
            [Property]
            public void IsUpdatedAccordingToTimeServiceForRunningTimeEntries(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                var tickSubject = new Subject<DateTimeOffset>();
                var tickObservable = tickSubject.AsObservable().Publish();
                tickObservable.Connect();
                TimeService.CurrentDateTimeObservable.Returns(tickObservable);
                TimeService.CurrentDateTime.Returns(now);
                ViewModel.Prepare(parameter);

                var newCurrentTime = now.AddHours(2);
                tickSubject.OnNext(newCurrentTime);

                ViewModel.StopTime.Should().BeSameDateAs(newCurrentTime);
            }
        }

        public sealed class ThePrepareMethod : EditDurationViewModelTest
        {
            [Property]
            public void SetsTheStartTime(DateTimeOffset now)
            {
                var start = now;
                var parameter = DurationParameter.WithStartAndStop(start, null);

                ViewModel.Prepare(parameter);

                ViewModel.StartTime.Should().Be(start);
            }

            [Property]
            public void SetsTheStartTimeToCurrentTimeIfParameterDoesNotHaveStartTime(DateTimeOffset now)
            {
                var start = now.AddHours(-2);
                var parameter = DurationParameter.WithStartAndStop(start, null);
                TimeService.CurrentDateTime.Returns(now);

                ViewModel.Prepare(parameter);

                ViewModel.StartTime.Should().BeSameDateAs(start);
            }

            [Property]
            public void SetsTheStopTimeToParameterStopTimeIfParameterHasStopTime(DateTimeOffset now)
            {
                var start = now.AddHours(-4);
                var stop = start.AddHours(2);
                var parameter = DurationParameter.WithStartAndStop(start, stop);

                ViewModel.Prepare(parameter);

                ViewModel.StartTime.Should().BeSameDateAs(start);
            }

            [Property]
            public void SubscribesToCurrentTimeObservableIfParameterDoesNotHaveStopTime(DateTimeOffset now)
            {
                var parameter = DurationParameter.WithStartAndStop(now, null);

                ViewModel.Prepare(parameter);

                TimeService.CurrentDateTimeObservable.Received().Subscribe(Arg.Any<AnonymousObserver<DateTimeOffset>>());
            }
        }

        public sealed class TheCloseCommand : EditDurationViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<DurationParameter>());
            }
            [Fact]
            public async Task ReturnsTheDefaultParameter()
            {
                var parameter = DurationParameter.WithStartAndStop(DateTimeOffset.UtcNow, null);
                ViewModel.Prepare(parameter);

                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Is(parameter));
            }
        }

        public sealed class TheSaveCommand : EditDurationViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.SaveCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<DurationParameter>());
            }

            [Property]
            public void ReturnsAValueThatReflectsTheChangesToDuration(DateTimeOffset start, DateTimeOffset? stop)
            {
                if (start >= stop) return;

                var now = DateTimeOffset.UtcNow;
                TimeService.CurrentDateTime.Returns(now);
                if (stop == null && start >= now) return;

                ViewModel.Prepare(DurationParameter.WithStartAndStop(start, stop));
                ViewModel.Duration = TimeSpan.FromMinutes(10);

                ViewModel.SaveCommand.ExecuteAsync().Wait();

                NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Is<DurationParameter>(
                    p => p.Start == ViewModel.StartTime && p.Stop == ViewModel.StopTime
                )).Wait();
            }
        }
    }
}

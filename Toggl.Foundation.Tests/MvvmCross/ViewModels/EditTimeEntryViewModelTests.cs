using System;
using System.Reactive.Linq;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Models;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Xunit;
using Task = System.Threading.Tasks.Task;
using static Toggl.Foundation.MvvmCross.Helper.Constants;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class EditTimeEntryViewModelTests
    {
        public abstract class EditTimeEntryViewModelTest : BaseViewModelTests<EditTimeEntryViewModel>
        {
            protected const long Id = 10;

            protected void ConfigureEditedTimeEntry(DateTimeOffset now, bool isRunning = false)
            {
                var te = TimeEntry.Builder.Create(Id)
                    .SetDescription("Something")
                    .SetStart(now.AddHours(-2))
                    .SetAt(now.AddHours(-2))
                    .SetWorkspaceId(11)
                    .SetUserId(12);

                if (!isRunning)
                    te = te.SetStop(now.AddHours(-1));
               
                var observable = Observable.Return(te.Build());

                DataSource.TimeEntries.GetById(Arg.Is(Id)).Returns(observable);
            }

            protected override EditTimeEntryViewModel CreateViewModel()
                => new EditTimeEntryViewModel(DataSource, NavigationService, TimeService);
        }

        public sealed class TheConstructor : EditTimeEntryViewModelTest
        {
            [Theory]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useNavigationService, bool useTimeService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var navigationService = useNavigationService ? NavigationService : null;
                var timeService = useTimeService ? TimeService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new EditTimeEntryViewModel(dataSource, navigationService, timeService);

                tryingToConstructWithEmptyParameters.ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class TheCloseCommand : EditTimeEntryViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel));
            }
        }

        public sealed class TheDeleteCommand : EditTimeEntryViewModelTest
        {
            [Fact]
            public void CallsDeleteOnDataSource()
            {
                ViewModel.DeleteCommand.Execute();

                DataSource.TimeEntries.Received().Delete(Arg.Is(ViewModel.Id));
            }
        }

        public sealed class TheSelectStartDateTimeCommandCommand : EditTimeEntryViewModelTest
        {
            [Property]
            public void CallsTheSelectDateTimeViewModelWithAMaxDateEqualToTheCurrentTimeWhenTheTimeEntryIsRunning(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                TimeService.CurrentDateTime.Returns(now);

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DatePickerParameters, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DatePickerParameters>())
                    .Returns(parameterToReturn);
                ConfigureEditedTimeEntry(now, true);
                ViewModel.Prepare(Id);

                ViewModel.SelectStartDateTimeCommand.ExecuteAsync().Wait();

                NavigationService
                    .Received()
                    .Navigate<DatePickerParameters, DateTimeOffset>(
                        typeof(SelectDateTimeViewModel),
                        Arg.Is<DatePickerParameters>(p => p.MaxDate == now));
            }

            [Property]
            public void CallsTheSelectDateTimeViewModelWithAMaxDateEqualToTheValueOfTheStopTimeIfTheTimeEntryIsComplete(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                TimeService.CurrentDateTime.Returns(now);

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DatePickerParameters, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DatePickerParameters>())
                    .Returns(parameterToReturn);
                ConfigureEditedTimeEntry(now);
                ViewModel.Prepare(Id);

                ViewModel.SelectStartDateTimeCommand.ExecuteAsync().Wait();

                NavigationService
                    .Received()
                    .Navigate<DatePickerParameters, DateTimeOffset>(
                        typeof(SelectDateTimeViewModel),
                        Arg.Is<DatePickerParameters>(p => p.MinDate == now.AddHours(-2)));
            }

            [Property]
            public void CallsTheSelectDateTimeViewModelWithAMinDateThatAllows999HoursOfDuration(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                TimeService.CurrentDateTime.Returns(now);

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DatePickerParameters, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DatePickerParameters>())
                    .Returns(parameterToReturn);
                ConfigureEditedTimeEntry(now);
                ViewModel.Prepare(Id);

                ViewModel.SelectStartDateTimeCommand.ExecuteAsync().Wait();

                NavigationService
                    .Received()
                    .Navigate<DatePickerParameters, DateTimeOffset>(
                        typeof(SelectDateTimeViewModel),
                        Arg.Is<DatePickerParameters>(p => p.MaxDate == p.MaxDate.AddHours(-MaxTimeEntryDurationInHours)));
            }

            [Property]
            public void SetsTheStartDateToTheValueReturnedByTheSelectDateTimeDialogViewModel(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DatePickerParameters, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DatePickerParameters>())
                    .Returns(parameterToReturn);
                ConfigureEditedTimeEntry(now);
                ViewModel.Prepare(Id);

                ViewModel.SelectStartDateTimeCommand.ExecuteAsync().Wait();

                ViewModel.StartTime.Should().Be(parameterToReturn);
            }
        }

        public sealed class TheEditDurationCommand : EditTimeEntryViewModelTest
        {
            [Property]
            public void SetsTheStartTimeToTheValueReturnedByTheSelectDateTimeDialogViewModelWhenEditingARunningTimeEntry(DateTimeOffset now)
            {
                var parameterToReturn = DurationParameter.WithStartAndStop(now.AddHours(-3), null);
                NavigationService
                    .Navigate<DurationParameter, DurationParameter>(typeof(EditDurationViewModel), Arg.Any<DurationParameter>())
                    .Returns(parameterToReturn);
                ConfigureEditedTimeEntry(now);
                ViewModel.Prepare(Id);

                ViewModel.EditDurationCommand.ExecuteAsync().Wait();

                ViewModel.StartTime.Should().Be(parameterToReturn.Start);
            }

            [Property]
            public void SetsTheStopTimeToTheValueReturnedByTheSelectDateTimeDialogViewModelWhenEditingACompletedTimeEntry(DateTimeOffset now)
            {
                var parameterToReturn = DurationParameter.WithStartAndStop(now.AddHours(-4), now.AddHours(-3));
                NavigationService
                    .Navigate<DurationParameter, DurationParameter>(typeof(EditDurationViewModel), Arg.Any<DurationParameter>())
                    .Returns(parameterToReturn);
                ConfigureEditedTimeEntry(now);
                ViewModel.Prepare(Id);

                ViewModel.EditDurationCommand.ExecuteAsync().Wait();

                ViewModel.StopTime.Should().Be(parameterToReturn.Stop);
            }
        }
    }
}

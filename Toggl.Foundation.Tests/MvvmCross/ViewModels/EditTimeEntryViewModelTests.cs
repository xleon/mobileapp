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
using Toggl.PrimeRadiant.Models;
using System.Linq;
using Toggl.Foundation.DataSources;
using System.Collections.Generic;
using FsCheck;

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

        public sealed class TheSelectTagsCommand : EditTimeEntryViewModelTest
        {
            [Property]
            public void NavigatesToTheSelectTagsViewModelPassingCurrentTagIds(NonNegativeInt[] nonNegativeInts)
            {
                var tagIds = nonNegativeInts.Select(i => (long)i.Get)
                    .Distinct();
                long id = 13;
                var timeEntry = Substitute.For<IDatabaseTimeEntry>();
                timeEntry.Id.Returns(id);
                timeEntry.TagIds.Returns(tagIds);
                DataSource.TimeEntries.GetById(Arg.Is(id)).Returns(Observable.Return(timeEntry));
                ViewModel.Prepare(id);
                ViewModel.Initialize().Wait();

                ViewModel.SelectTagsCommand.ExecuteAsync().Wait();

                NavigationService
                    .Received()
                    .Navigate<long[], long[]>(
                        Arg.Is(typeof(SelectTagsViewModel)),
                        Arg.Is<long[]>(ids => ids.SequenceEqual(tagIds)))
                    .Wait();
            }

            [Property]
            public void QueriesTheDataSourceForReturnedTagIds(
                NonEmptyArray<NonNegativeInt> nonNegativeInts, long[] otherIds)
            {
                var tagIds = nonNegativeInts.Get
                    .Select(i => (long)i.Get)
                    .ToArray();
                var tags = tagIds.Select(createTag);
                var otherTags = otherIds.Select(createTag);
                DataSource.Tags.GetAll(Arg.Any<Func<IDatabaseTag, bool>>())
                    .Returns(Observable.Return(tags));
                NavigationService
                    .Navigate<long[], long[]>(Arg.Is(typeof(SelectTagsViewModel)), Arg.Any<long[]>())
                    .Returns(Task.FromResult(tagIds));
                ViewModel.Initialize().Wait();

                ViewModel.SelectTagsCommand.ExecuteAsync().Wait();

                DataSource.Tags.Received()
                    .GetAll(Arg.Is<Func<IDatabaseTag, bool>>(
                        func => ensureFuncWorksAsExpected(func, tags, otherTags)))
                    .Wait();
            }

            private bool ensureFuncWorksAsExpected(
                Func<IDatabaseTag, bool> func,
                IEnumerable<IDatabaseTag> tags,
                IEnumerable<IDatabaseTag> otherTags)
            {
                var tagIdHashSet = new HashSet<long>(tags.Select(tag => tag.Id));
                foreach (var tag in tags)
                    if (!func(tag))
                        return false;

                foreach (var otherTag in otherTags)
                {
                    if (tagIdHashSet.Contains(otherTag.Id))
                        continue;
                    if (func(otherTag))
                        return false;
                }

                return true;
            }

            [Property]
            public void SetsTheReturnedTags(NonEmptyArray<NonNegativeInt> nonNegativeInts)
            {
                var tagIds = nonNegativeInts.Get
                    .Select(i => (long)i.Get)
                    .ToArray();
                var tags = tagIds.Select(createTag);
                var tagNames = tags.Select(tag => tag.Name);
                ViewModel.Initialize().Wait();
                DataSource.Tags.GetAll(Arg.Any<Func<IDatabaseTag, bool>>())
                    .Returns(Observable.Return(tags));
                NavigationService
                    .Navigate<long[], long[]>(Arg.Is(typeof(SelectTagsViewModel)), Arg.Any<long[]>())
                    .Returns(Task.FromResult(tagIds));

                ViewModel.SelectTagsCommand.ExecuteAsync().Wait();

                ViewModel.Tags.Should()
                         .HaveCount(tags.Count()).And
                         .OnlyContain(tag => tagNames.Contains(tag));
            }

            private IDatabaseTag createTag(long id)
            {
                var tag = Substitute.For<IDatabaseTag>();
                tag.Id.Returns(id);
                tag.Name.Returns($"Tag{id}");
                return tag;
            }
        }
    }
}

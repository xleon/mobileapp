using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.Autocomplete.Suggestions;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Toggl.PrimeRadiant.Models;
using Xunit;
using static Toggl.Foundation.MvvmCross.Helper.Constants;
using TextFieldInfo = Toggl.Foundation.Autocomplete.TextFieldInfo;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public sealed class StartTimeEntryViewModelTests
    {
        public abstract class StartTimeEntryViewModelTest : BaseViewModelTests<StartTimeEntryViewModel>
        {
            protected const long ProjectId = 10;
            protected const string ProjectName = "Toggl";
            protected const string ProjectColor = "#F41F19";
            protected const string Description = "Testing Toggl mobile apps";

            protected IAutocompleteProvider AutocompleteProvider { get; } = Substitute.For<IAutocompleteProvider>();

            protected StartTimeEntryViewModelTest()
            {
                DataSource.AutocompleteProvider.Returns(AutocompleteProvider);
            }

            protected override StartTimeEntryViewModel CreateViewModel()
                => new StartTimeEntryViewModel(DataSource, TimeService, NavigationService);
        }

        public sealed class TheConstructor : StartTimeEntryViewModelTest
        {
            [Theory]
            [ClassData(typeof(ThreeParameterConstructorTestData))]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useDataSource, bool useTimeService, 
                bool useNavigationService)
            {
                var dataSource = useDataSource ? DataSource : null;
                var timeService = useTimeService ? TimeService : null;
                var navigationService = useNavigationService ? NavigationService : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new StartTimeEntryViewModel(dataSource, timeService, navigationService);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public sealed class ThePrepareMethod : StartTimeEntryViewModelTest
        {
            [Fact]
            public void SetsTheDateAccordingToTheDateParameterReceived()
            {
                var date = DateTimeOffset.UtcNow;

                ViewModel.Prepare(date);

                ViewModel.StartTime.Should().BeSameDateAs(date);
            }
        }

        public sealed class TheBackCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.BackCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }

        public sealed class TheToggleBillableCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public void TogglesTheIsBillableProperty()
            {
                var expected = !ViewModel.IsBillable;

                ViewModel.ToggleBillableCommand.Execute();

                ViewModel.IsBillable.Should().Be(expected);
            }
        }

        public sealed class TheChangeStartTimeCommand : StartTimeEntryViewModelTest
        {
            [Property]
            public void CallsTheSelectDateTimeViewModelWithAMinDateThatAllowsUpTo999HoursOfDuration(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                TimeService.CurrentDateTime.Returns(now);

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DatePickerParameters, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DatePickerParameters>())
                    .Returns(parameterToReturn);
                ViewModel.Prepare(now);

                ViewModel.ChangeStartTimeCommand.ExecuteAsync().Wait();


                NavigationService
                    .Received()
                    .Navigate<DatePickerParameters, DateTimeOffset>(
                        typeof(SelectDateTimeViewModel),
                        Arg.Is<DatePickerParameters>(p => p.MinDate == now.AddHours(-MaxTimeEntryDurationInHours)));
            }

            [Property]
            public void CallsTheSelectDateTimeViewModelWithAMaxDateEqualToTheCurrentDate(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                TimeService.CurrentDateTime.Returns(now);

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DatePickerParameters, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DatePickerParameters>())
                    .Returns(parameterToReturn);
                ViewModel.Prepare(now);

                ViewModel.ChangeStartTimeCommand.ExecuteAsync().Wait();


                NavigationService
                    .Received()
                    .Navigate<DatePickerParameters, DateTimeOffset>(
                        typeof(SelectDateTimeViewModel),
                        Arg.Is<DatePickerParameters>(p => p.MaxDate == now));
            }

            [Property]
            public void SetsTheStartDateToTheValueReturnedByTheSelectDateTimeDialogViewModel(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;

                var parameterToReturn = now.AddHours(-2);
                NavigationService
                    .Navigate<DateTimeOffset, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DateTimeOffset>())
                    .Returns(parameterToReturn);
                ViewModel.Prepare(now);

                ViewModel.ChangeStartTimeCommand.ExecuteAsync().Wait();

                ViewModel.StartTime.Should().Be(parameterToReturn);
            }

            [Property]
            public void SetsTheIsEditingStartDateToTrueWhileTheViewDoesNotReturnAndThenSetsItBackToFalse(DateTimeOffset now)
            {
                if (DateTimeOffset.MinValue.AddHours(MaxTimeEntryDurationInHours) <= now ||
                    DateTimeOffset.MaxValue.AddHours(-1) >= now) return;
                
                var parameterToReturn = now.AddHours(-2);
                var tcs = new TaskCompletionSource<DateTimeOffset>();
                NavigationService
                    .Navigate<DateTimeOffset, DateTimeOffset>(typeof(SelectDateTimeViewModel), Arg.Any<DateTimeOffset>())
                    .Returns(tcs.Task);
                ViewModel.Prepare(now);

                var toWait = ViewModel.ChangeStartTimeCommand.ExecuteAsync();
                ViewModel.IsEditingStartDate.Should().BeTrue();
                tcs.SetResult(parameterToReturn);
                toWait.Wait();

                ViewModel.IsEditingStartDate.Should().BeFalse();
            }
        }   

        public sealed class TheToggleProjectSuggestionsCommandCommand : StartTimeEntryViewModelTest
        {
            public TheToggleProjectSuggestionsCommandCommand()
            {
                var items = ProjectSuggestion.FromProjectsPrependingEmpty(Enumerable.Empty<IDatabaseProject>());
                AutocompleteProvider
                    .Query(Arg.Is<TextFieldInfo>(info => info.Text.Contains("@")))
                    .Returns(Observable.Return(items));
            }

            [Fact]
            public void StartProjectSuggestionEvenIfTheProjectHasAlreadyBeenSelected()
            {
                ViewModel.Prepare(DateTimeOffset.UtcNow);
                ViewModel.TextFieldInfo = new TextFieldInfo(
                    Description, Description.Length,
                    ProjectId, ProjectName, ProjectColor);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeTrue();
            }

            [Fact]
            public void SetsTheIsSuggestingProjectsPropertyToTrueIfNotInProjectSuggestionMode()
            {
                ViewModel.Prepare(DateTimeOffset.UtcNow);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeTrue();
            }

            [Fact]
            public void AddsAnAtSymbolAtTheEndOfTheQueryInOrderToStartProjectSuggestionMode()
            {
                const string description = "Testing Toggl Apps";
                var expected = $"{description}@";
                ViewModel.Prepare(DateTimeOffset.UtcNow);
                ViewModel.TextFieldInfo = new TextFieldInfo(description, description.Length);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.TextFieldInfo.Text.Should().Be(expected);
            }

            [Theory]
            [InlineData("@")]
            [InlineData("@somequery")]
            [InlineData("@some query")]
            [InlineData("@some query@query")]
            [InlineData("Testing Toggl Apps @")]
            [InlineData("Testing Toggl Apps @somequery")]
            [InlineData("Testing Toggl Apps @some query")]
            [InlineData("Testing Toggl Apps @some query@query")]
            [InlineData("Testing Toggl Apps@")]
            [InlineData("Testing Toggl Apps@somequery")]
            [InlineData("Testing Toggl Apps@some query")]
            [InlineData("Testing Toggl Apps@some query@query")]
            public void SetsTheIsSuggestingProjectsPropertyToFalseIfInProjectSuggestionMode(string description)
            {
                ViewModel.Prepare(DateTimeOffset.UtcNow);
                ViewModel.TextFieldInfo = new TextFieldInfo(description, description.Length);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.IsSuggestingProjects.Should().BeFalse();
            }

            [Theory]
            [InlineData("@", "")]
            [InlineData("@somequery", "")]
            [InlineData("@some query", "")]
            [InlineData("@some query@query", "")]
            [InlineData("Testing Toggl Apps @", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @somequery", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @some query", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps @some query@query", "Testing Toggl Apps ")]
            [InlineData("Testing Toggl Apps@", "Testing Toggl Apps")]
            [InlineData("Testing Toggl Apps@somequery", "Testing Toggl Apps")]
            [InlineData("Testing Toggl Apps@some query", "Testing Toggl Apps")]
            [InlineData("Testing Toggl Apps@some query@query", "Testing Toggl Apps")]
            public void RemovesTheAtSymbolFromTheDescriptionTextIfAlreadyInProjectSuggestionMode(
                string description, string expected)
            {
                ViewModel.Prepare(DateTimeOffset.UtcNow);
                ViewModel.TextFieldInfo = new TextFieldInfo(description, description.Length);

                ViewModel.ToggleProjectSuggestionsCommand.Execute();

                ViewModel.TextFieldInfo.Text.Should().Be(expected);
            }
        }

        public sealed class TheChangeDurationCommandCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task SetsTheStartDateToTheValueReturnedByTheSelectDateTimeDialogViewModel()
            {
                var now = DateTimeOffset.UtcNow;
                var parameterToReturn = DurationParameter.WithStartAndStop(now.AddHours(-2), null);
                NavigationService
                    .Navigate<DurationParameter, DurationParameter>(typeof(EditDurationViewModel), Arg.Any<DurationParameter>())
                    .Returns(parameterToReturn);
                ViewModel.Prepare(now);

                await ViewModel.ChangeDurationCommand.ExecuteAsync();

                ViewModel.StartTime.Should().Be(parameterToReturn.Start);
            }

            [Fact]
            public async Task SetsTheIsEditingDurationDateToTrueWhileTheViewDoesNotReturnAndThenSetsItBackToFalse()
            {
                var now = DateTimeOffset.UtcNow;
                var parameterToReturn = DurationParameter.WithStartAndStop(now.AddHours(-2), null);
                var tcs = new TaskCompletionSource<DurationParameter>();
                NavigationService
                    .Navigate<DurationParameter, DurationParameter>(typeof(EditDurationViewModel), Arg.Any<DurationParameter>())
                    .Returns(tcs.Task);
                ViewModel.Prepare(now);

                var toWait = ViewModel.ChangeDurationCommand.ExecuteAsync();
                ViewModel.IsEditingDuration.Should().BeTrue();
                tcs.SetResult(parameterToReturn);
                await toWait;

                ViewModel.IsEditingDuration.Should().BeFalse();
            }
        }

        public sealed class TheDoneCommand : StartTimeEntryViewModelTest
        {
            [Fact]
            public async Task StartsANewTimeEntry()
            {
                var date = DateTimeOffset.UtcNow;
                var description = "Testing Toggl apps";

                ViewModel.Prepare(date);
                ViewModel.TextFieldInfo = new TextFieldInfo(description, 0);
                ViewModel.DoneCommand.Execute();

                await DataSource.TimeEntries.Received().Start(Arg.Is<StartTimeEntryDTO>(dto =>
                    dto.StartTime == date &&
                    dto.Description == description &&
                    dto.Billable == false &&
                    dto.ProjectId == null
                ));
            }

            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.DoneCommand.ExecuteAsync();

                await NavigationService.Received().Close(ViewModel);
            }
        }

        public sealed class TheSelectSuggestionCommand
        {
            public abstract class SelectSuggestionTest<TSuggestion> : StartTimeEntryViewModelTest
                where TSuggestion : AutocompleteSuggestion
            {
                protected IDatabaseProject Project { get; }
                protected IDatabaseTimeEntry TimeEntry { get; }

                protected abstract TSuggestion Suggestion { get; }

                protected SelectSuggestionTest()
                {
                    Project = Substitute.For<IDatabaseProject>();
                    Project.Id.Returns(ProjectId);
                    Project.Name.Returns(ProjectName);
                    Project.Color.Returns(ProjectColor);
                    TimeEntry = Substitute.For<IDatabaseTimeEntry>();
                    TimeEntry.Description.Returns(Description);
                    TimeEntry.Project.Returns(Project);
                }
            }

            public sealed class WhenSelectingATimeEntrySuggestion : SelectSuggestionTest<TimeEntrySuggestion>
            {
                protected override TimeEntrySuggestion Suggestion { get; }

                public WhenSelectingATimeEntrySuggestion()
                {
                    Suggestion = new TimeEntrySuggestion(TimeEntry);
                }

                [Fact]
                public void SetsTheTextFieldInfoTextToTheValueOfTheSuggestedDescription()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.Text.Should().Be(Description);
                }

                [Fact]
                public void SetsTheProjectIdToTheSuggestedProjectId()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectId.Should().Be(ProjectId);
                }

                [Fact]
                public void SetsTheProjectNameToTheSuggestedProjectName()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectName.Should().Be(ProjectName);
                }

                [Fact]
                public void SetsTheProjectColorToTheSuggestedProjectColor()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectColor.Should().Be(ProjectColor);
                }
            }

            public sealed class WhenSelectingAProjectSuggestion : SelectSuggestionTest<ProjectSuggestion>
            {
                protected override ProjectSuggestion Suggestion { get; }

                public WhenSelectingAProjectSuggestion()
                {
                    Suggestion = new ProjectSuggestion(Project);

                    ViewModel.TextFieldInfo = new TextFieldInfo("Something @togg", 15);
                }

                [Fact]
                public void RemovesTheProjectQueryFromTheTextFieldInfo()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.Text.Should().Be("Something ");
                }

                [Fact]
                public void SetsTheProjectIdToTheSuggestedProjectId()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectId.Should().Be(ProjectId);
                }

                [Fact]
                public void SetsTheProjectNameToTheSuggestedProjectName()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectName.Should().Be(ProjectName);
                }

                [Fact]
                public void SetsTheProjectColorToTheSuggestedProjectColor()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.ProjectColor.Should().Be(ProjectColor);
                }
            }

            public sealed class WhenSelectingAQuerySymbolSuggestion : SelectSuggestionTest<QuerySymbolSuggestion>
            {
                protected override QuerySymbolSuggestion Suggestion { get; } = QuerySymbolSuggestion.Suggestions.First();

                [Fact]
                public void SetsTheTextToTheQuerySymbolSelected()
                {
                    ViewModel.SelectSuggestionCommand.Execute(Suggestion);

                    ViewModel.TextFieldInfo.Text.Should().Be(Suggestion.Symbol);
                }
            }
        }

        public sealed class TheSuggestionsProperty : StartTimeEntryViewModelTest
        {
            [Fact]
            public void IsClearedWhenThereAreNoWordsToQuery()
            {
                ViewModel.TextFieldInfo = new TextFieldInfo("", 0);

                ViewModel.Suggestions.Should().HaveCount(0);
            }
        }
    }
}

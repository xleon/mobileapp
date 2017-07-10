using System;
using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class TimeEntriesLogViewModelTests
    {
        public class TimeEntriesLogViewModelTest : BaseViewModelTests<TimeEntriesLogViewModel>
        {
            protected override TimeEntriesLogViewModel CreateViewModel()
                => new TimeEntriesLogViewModel(DataSource);
        }

        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentsIsNull()
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new TimeEntriesLogViewModel(null);

                tryingToConstructWithEmptyParameters
                    .ShouldThrow<ArgumentNullException>();
            }
        }

        public class TheEmptyStateTitleProperty : TimeEntriesLogViewModelTest
        {
            [Fact]
            public void ReturnsTheWelcomeStringIfTheIsWelcomePropertyIsTrue()
            {
                ViewModel.IsWelcome = true;

                ViewModel.EmptyStateTitle.Should().Be(Resources.TimeEntriesLogEmptyStateWelcomeTitle);
            }

            [Fact]
            public void ReturnsTheDefaultStringIfTheIsWelcomePropertyIsFalse()
            {
                ViewModel.IsWelcome = false;

                ViewModel.EmptyStateTitle.Should().Be(Resources.TimeEntriesLogEmptyStateTitle);
            }
        }

        public class TheEmptyStateTextProperty : TimeEntriesLogViewModelTest
        {
            [Fact]
            public void ReturnsTheWelcomeStringIfTheIsWelcomePropertyIsTrue()
            {
                ViewModel.IsWelcome = true;

                ViewModel.EmptyStateText.Should().Be(Resources.TimeEntriesLogEmptyStateWelcomeText);
            }

            [Fact]
            public void ReturnsTheDefaultStringIfTheIsWelcomePropertyIsFalse()
            {
                ViewModel.IsWelcome = false;

                ViewModel.EmptyStateText.Should().Be(Resources.TimeEntriesLogEmptyStateText);
            }
        }

        public class TheTimeEntriesProperty
        {
            [Property]
            public Property ShouldBeOrderedAfterInitialization()
            {
                var generator = ViewModelGenerators.ForTimeEntriesLogViewModel(_ => true).ToArbitrary();
                return Prop.ForAll(generator, viewModel =>
                {
                    viewModel.Initialize().Wait();

                    for (int i = 1; i < viewModel.TimeEntries.Count(); i++)
                    {
                        var dateTime1 = viewModel.TimeEntries.ElementAt(i - 1).Key;
                        var dateTime2 = viewModel.TimeEntries.ElementAt(i).Key;
                        dateTime1.Should().BeAfter(dateTime2);
                    }
                });
            }

            [Property]
            public Property ShouldNotHaveEmptyGroups()
            {
                var generator = ViewModelGenerators.ForTimeEntriesLogViewModel(_ => true).ToArbitrary();
                return Prop.ForAll(generator, viewModel =>
                {
                    viewModel.Initialize().Wait();

                    foreach (var grouping in viewModel.TimeEntries)
                    {
                        grouping.Count().Should().BeGreaterOrEqualTo(1);
                    }
                });
            }

            [Property]
            public Property ShouldHaveOrderedGroupsAfterInitialization()
            {
                var generator =
                    ViewModelGenerators
                        .ForTimeEntriesLogViewModel(m => m == DateTime.UtcNow.Month).ToArbitrary();

                return Prop.ForAll(generator, viewModel =>
                {
                    viewModel.Initialize().Wait();
                    foreach (var grouping in viewModel.TimeEntries)
                    {
                        for (int i = 1; i < grouping.Count(); i++)
                        {
                            var dateTime1 = grouping.ElementAt(i - 1).Start;
                            var dateTime2 = grouping.ElementAt(i).Start;
                            dateTime1.Should().BeOnOrAfter(dateTime2);
                        }
                    }
                });
            }
        }
    }
}

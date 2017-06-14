using System;
using System.Linq;
using FluentAssertions;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class OnboardingViewModelTests
    {
        public class TheCurrentPageProperty : BaseViewModelTests<OnboardingViewModel>
        {
            [Fact]
            public void DoesNotAllowUsersToSetItsValueToAValueGreaterThanTheNumberOfPagesMinusOne()
            {
                ViewModel.CurrentPage = OnboardingViewModel.TrackPage;

                ViewModel.CurrentPage = ViewModel.NumberOfPages;
                ViewModel.CurrentPage = ViewModel.NumberOfPages + 1;

                ViewModel.CurrentPage.Should().Be(OnboardingViewModel.TrackPage);
            }

            [Fact]
            public void DoesNotAllowUsersToSetItsValueToANegativeNumber()
            {
                ViewModel.CurrentPage = OnboardingViewModel.TrackPage;

                ViewModel.CurrentPage = -1;

                ViewModel.CurrentPage.Should().Be(OnboardingViewModel.TrackPage);
            }
        }

        public class TheIsNextVisibleProperty : BaseViewModelTests<OnboardingViewModel>
        {
            [Theory]
            [InlineData(OnboardingViewModel.TrackPage)]
            [InlineData(OnboardingViewModel.LogPage)]
            [InlineData(OnboardingViewModel.SummaryPage)]
            [InlineData(OnboardingViewModel.LoginPage)]
            public void ReturnsTrueInAllPagesButTheLastOne(int page)
            {
                ViewModel.CurrentPage = page;

                var expected = page != ViewModel.NumberOfPages - 1;
                ViewModel.IsNextVisible.Should().Be(expected);
            }
        }

        public class TheIsPreviousVisibleProperty : BaseViewModelTests<OnboardingViewModel>
        {
            [Theory]
            [InlineData(OnboardingViewModel.TrackPage)]
            [InlineData(OnboardingViewModel.LogPage)]
            [InlineData(OnboardingViewModel.SummaryPage)]
            [InlineData(OnboardingViewModel.LoginPage)]
            public void ReturnsTrueInAllPagesButTheFirstOne(int page)
            {
                ViewModel.CurrentPage = page;

                var expected = page != 0;
                ViewModel.IsPreviousVisible.Should().Be(expected);
            }
        }

        public class TheSkipCommand : BaseViewModelTests<OnboardingViewModel>
        {
            [Fact]
            public void GoesStraightToTheLastPage()
            {
                ViewModel.SkipCommand.Execute();

                ViewModel.CurrentPage.Should().Be(OnboardingViewModel.LoginPage);
            }
        }

        public class TheNextCommand : BaseViewModelTests<OnboardingViewModel>
        {
            [Fact]
            public void AdvancesToTheNextPage()
            {
                ViewModel.CurrentPage = OnboardingViewModel.TrackPage;

                ViewModel.NextCommand.Execute();

                ViewModel.CurrentPage.Should().Be(OnboardingViewModel.LogPage);
            }

            [Fact]
            public void CannotBeExecutedWhenOnTheLastPage()
            {
                ViewModel.CurrentPage = OnboardingViewModel.LoginPage;

                ViewModel.NextCommand.CanExecute().Should().BeFalse();
            }
        }

        public class ThePreviousCommand : BaseViewModelTests<OnboardingViewModel>
        {
            [Fact]
            public void ReturnsToThePreviousPage()
            {
                ViewModel.CurrentPage = OnboardingViewModel.LogPage;

                ViewModel.PreviousCommand.Execute();

                ViewModel.CurrentPage.Should().Be(OnboardingViewModel.TrackPage);
            }

            [Fact]
            public void CannotBeExecutedWhenOnTheFirstPage()
            {
                ViewModel.CurrentPage = OnboardingViewModel.TrackPage;

                ViewModel.PreviousCommand.CanExecute().Should().BeFalse();
            }
        }
    }
}

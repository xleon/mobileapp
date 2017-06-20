using FluentAssertions;
﻿using System.Threading.Tasks;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;
using Toggl.Foundation.MvvmCross.Parameters;

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

        public class TheIsFirstPageProperty : BaseViewModelTests<OnboardingViewModel>
        {
            [Theory]
            [InlineData(OnboardingViewModel.TrackPage)]
            [InlineData(OnboardingViewModel.LogPage)]
            [InlineData(OnboardingViewModel.SummaryPage)]
            [InlineData(OnboardingViewModel.LoginPage)]
            public void OnlyReturnsTrueForTheFirstPage(int page)
            {
                ViewModel.CurrentPage = page;

                var expected = page == 0;
                ViewModel.IsFirstPage.Should().Be(expected);
            }
        }

        public class TheIsLastPageProperty : BaseViewModelTests<OnboardingViewModel>
        {
            [Theory]
            [InlineData(OnboardingViewModel.TrackPage)]
            [InlineData(OnboardingViewModel.LogPage)]
            [InlineData(OnboardingViewModel.SummaryPage)]
            [InlineData(OnboardingViewModel.LoginPage)]
            public void OnlyReturnsTrueInThePageWhoseIndexEqualsToTheNumberOfPagesMinusOne(int page)
            {
                ViewModel.CurrentPage = page;

                var expected = page == ViewModel.NumberOfPages - 1;
                ViewModel.IsLastPage.Should().Be(expected);
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

        public class TheLoginCommand : BaseViewModelTests<OnboardingViewModel>
        {
            [Fact]
            public async Task RequestsTheLoginViewModelFromTheNavigationService()
            {
                await ViewModel.LoginCommand.ExecuteAsync();

                await NavigationService.Received().Navigate<LoginViewModel, LoginParameter>(Arg.Any<LoginParameter>());
            }
        }
    }
}

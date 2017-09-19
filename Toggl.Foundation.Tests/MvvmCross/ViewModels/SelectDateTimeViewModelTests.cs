using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class SelectDateTimeViewModelTests
    {
        public class SelectDateTimeDialogViewModelTest : BaseViewModelTests<SelectDateTimeViewModel>
        {
            protected override SelectDateTimeViewModel CreateViewModel()
                => new SelectDateTimeViewModel(NavigationService);
        }

        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameter
                    = () => new SelectDateTimeViewModel(null);

                tryingToConstructWithEmptyParameter.ShouldThrow<ArgumentNullException>();
            }
        }
        
        public class TheCloseCommand : SelectDateTimeDialogViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<DateTimeOffset>());
            }

            [Property]
            public void ReturnsTheDefaultParameter(DateTimeOffset parameter)
            {
                ViewModel.Prepare(parameter);

                ViewModel.CloseCommand.ExecuteAsync().Wait();

                NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Is(parameter)).Wait();
            }
        }

        public class TheSaveCommand : SelectDateTimeDialogViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<DateTimeOffset>());
            }

            [Property]
            public void ReturnsAValueThatReflectsTheChangesToDuration(DateTimeOffset dateTimeOffset)
            {
                ViewModel.Prepare(DateTimeOffset.UtcNow);
                ViewModel.DateTimeOffset = dateTimeOffset;

                ViewModel.SaveCommand.ExecuteAsync().Wait();
                
                NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Is<DateTimeOffset>(p => p == dateTimeOffset))
                    .Wait();
            }
        }
    }
}

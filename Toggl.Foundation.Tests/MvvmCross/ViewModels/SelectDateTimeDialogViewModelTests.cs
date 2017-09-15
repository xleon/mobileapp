using System;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public class SelectDateTimeDialogViewModelTests
    {
        public class SelectDateTimeDialogViewModelTest : BaseViewModelTests<SelectDateTimeDialogViewModel>
        {
            protected override SelectDateTimeDialogViewModel CreateViewModel()
                => new SelectDateTimeDialogViewModel(NavigationService);
        }

        public class TheConstructor
        {
            [Fact]
            public void ThrowsIfTheArgumentIsNull()
            {
                Action tryingToConstructWithEmptyParameter
                    = () => new SelectDateTimeDialogViewModel(null);

                tryingToConstructWithEmptyParameter.ShouldThrow<ArgumentNullException>();
            }
        }
        
        public class TheCloseCommand : SelectDateTimeDialogViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<DateParameter>());
            }

            [Fact]
            public async Task ReturnsTheDefaultParameter()
            {
                var parameter = DateParameter.WithDate(new DateTime(2017, 1, 2));
                ViewModel.Prepare(parameter);

                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Is(parameter));
            }
        }

        public class TheSaveCommand : SelectDateTimeDialogViewModelTest
        {
            [Fact]
            public async Task ClosesTheViewModel()
            {
                await ViewModel.CloseCommand.ExecuteAsync();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<DateParameter>());
            }

            [Property]
            public void ReturnsAValueThatReflectsTheChangesToDuration(DateTimeOffset dateTimeOffset)
            {
                ViewModel.Prepare(DateParameter.WithDate(DateTimeOffset.UtcNow));
                ViewModel.DateTimeOffset = dateTimeOffset;

                ViewModel.SaveCommand.ExecuteAsync().Wait();
                
                NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Is<DateParameter>(p => p.GetDate() == dateTimeOffset))
                    .Wait();
            }
        }
    }
}

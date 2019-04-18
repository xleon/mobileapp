using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Xunit;
using System.Reactive.Linq;
using Toggl.Core.Tests.Generators;
using Toggl.Shared;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectColorViewModelTests
    {
        public abstract class SelectColorViewModelTest : BaseViewModelTests<SelectColorViewModel>
        {
            protected override SelectColorViewModel CreateViewModel()
                => new SelectColorViewModel(NavigationService, RxActionFactory);
        }

        public sealed class TheConstructor : SelectColorViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool useRxActionFactory)
            {
                var navigationService = useNavigationService ? NavigationService : null;
                var rxActionFactory = useRxActionFactory ? RxActionFactory : null;

                Action tryingToConstructWithEmptyParameters =
                    () => new SelectColorViewModel(navigationService, rxActionFactory);

                tryingToConstructWithEmptyParameters
                    .Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheSelectColorAction : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ChangesTheSelectedColor()
            {
                var initiallySelectedColor = Colors.DefaultProjectColors.First();
                var colorToSelect = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(initiallySelectedColor, true);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                ViewModel.SelectColor.Execute(colorToSelect);

                observer.Messages
                    .Select( m => m.Value.Value)
                    .Last()
                    .Single(c => c.Selected).Color.Should().BeEquivalentTo(colorToSelect);
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedColorIfCustomColorsAreNotAllowed()
            {
                var initiallySelectedColor = Colors.DefaultProjectColors.First();
                var colorToSelect = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(initiallySelectedColor, false);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                ViewModel.SelectColor.Execute(colorToSelect);

                await NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Is<Color>(c => c == colorToSelect));
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotReturnIfCustomColorsAreAllowed()
            {
                var initiallySelectedColor = Colors.DefaultProjectColors.First();
                var colorToSelect = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(initiallySelectedColor, true);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                ViewModel.SelectColor.Execute(colorToSelect);

                await NavigationService.DidNotReceive()
                    .Close(Arg.Is(ViewModel), Arg.Any<Color>());
            }
        }

        public sealed class ThePrepareCommand : SelectColorViewModelTest
        {

            [Fact, LogIfTooSlow]
            public void AddsFourteenItemsToTheListOfSelectableColorsIfTheUserIsNotPro()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, false);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                observer.Messages
                    .Select( m => m.Value.Value)
                    .Last()
                    .Should().HaveCount(14);
            }

            [Fact, LogIfTooSlow]
            public void AddsFifteenItemsToTheListOfSelectableColorsIfTheUserIsPro()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, true);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                observer.Messages
                    .Select( m => m.Value.Value)
                    .Last()
                    .Should().HaveCount(15);
            }

            [Fact, LogIfTooSlow]
            public void SelectsTheColorPassedAsTheParameter()
            {
                var passedColor = Colors.DefaultProjectColors.Skip(3).First();
                var parameters = ColorParameters.Create(passedColor, false);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                observer.Messages
                    .Select( m => m.Value.Value)
                    .Last()
                    .Single(c => c.Selected).Color.Should().Be(passedColor);
            }

            [Fact, LogIfTooSlow]
            public void SelectsTheFirstColorIfThePassedColorIsNotPartOfTheDefaultColorsAndWorkspaceIsNotPro()
            {
                var someColor = new Color(23, 45, 125);
                var expected = Colors.DefaultProjectColors.First();
                var parameters = ColorParameters.Create(someColor, false);

                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                observer.Messages
                    .Select( m => m.Value.Value)
                    .Last()
                    .Single(c => c.Selected).Color.Should().Be(expected);
            }

            [Fact, LogIfTooSlow]
            public void SelectsThePassedColorIfThePassedColorIsNotPartOfTheDefaultColorsAndWorkspaceIsPro()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, true);
                var observer = TestScheduler.CreateObserver<IEnumerable<SelectableColorViewModel>>();
                ViewModel.SelectableColors.Subscribe(observer);

                ViewModel.Prepare(parameters);

                observer.Messages
                    .Select( m => m.Value.Value)
                    .Last()
                    .Single(c => c.Selected).Color.Should().Be(someColor);
            }
        }

        public class TheCloseCommand : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                ViewModel.Close.Execute();
                TestScheduler.Start();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<Color>());
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheDefaultParameter()
            {
                var color = Colors.DefaultProjectColors.Last();
                var parameters = ColorParameters.Create(color, true);
                ViewModel.Prepare(parameters);

                ViewModel.Close.Execute();
                TestScheduler.Start();

                NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Is(color)).Wait();
            }
        }

        public sealed class TheSaveCommand : SelectColorViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                ViewModel.Close.Execute();
                TestScheduler.Start();

                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<Color>());
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedColor()
            {
                var someColor = new Color(23, 45, 125);
                var parameters = ColorParameters.Create(someColor, true);
                ViewModel.Prepare(parameters);
                var expected = Colors.DefaultProjectColors.First();
                ViewModel.SelectColor.Execute(expected);

                ViewModel.Save.Execute();
                TestScheduler.Start();

                await NavigationService.Received()
                    .Close(Arg.Is(ViewModel), Arg.Is<Color>(c => c == expected));
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using MvvmCross.Navigation;
using NSubstitute;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Tests.Generators;
using Xunit;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public enum Foo
    {
        OptionA,
        OptionB,
        OptionC
    }

    public sealed class SelectFromListViewModelTests
    {
        public sealed class SelectFooViewModel : SelectFromListViewModel<Foo>
        {
            public SelectFooViewModel(IMvxNavigationService navigationService) : base(navigationService) { }
        }

        public abstract class SelectFromListViewModelTest : BaseViewModelTests<SelectFooViewModel>
        {
            protected List<SelectableItem<Foo>> Items = new List<SelectableItem<Foo>>
            {
                new SelectableItem<Foo> { Title = "Option A", Value = Foo.OptionA },
                new SelectableItem<Foo> { Title = "Option B", Value = Foo.OptionB },
                new SelectableItem<Foo> { Title = "Option C", Value = Foo.OptionC },
            };

            protected override SelectFooViewModel CreateViewModel()
                => new SelectFooViewModel(NavigationService);

            protected void Prepare()
            {
                var parameters = new SelectFromListParameters<Foo>
                {
                    Items = Items,
                    SelectedIndex = 0,
                };

                ViewModel.Prepare(parameters);
            }
        }

        public sealed class TheConstructor : SelectFromListViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(bool useNavigationService)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new SelectFooViewModel(useNavigationService ? NavigationService : null);

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheItemsProperty : SelectFromListViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void ReturnsTheSelectedItem()
            {
                Prepare();

                ViewModel.Items.Should().BeEquivalentTo(Items);
            }
        }

        public sealed class TheBackAction : SelectFromListViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModel()
            {
                Prepare();

                await ViewModel.BackAction.Execute(Unit.Default);
                await NavigationService.Received().Close(Arg.Is(ViewModel), Arg.Any<Foo>());
            }

            [Fact, LogIfTooSlow]
            public async Task ReturnsTheSelectedItem()
            {
                Prepare();

                var index = 2;
                await ViewModel.SelectItemAction.Execute(index);
                await ViewModel.BackAction.Execute(Unit.Default);

                await NavigationService.Received().Close(Arg.Is(ViewModel), Foo.OptionC);
            }
        }

        public sealed class TheSelectItemAction : SelectFromListViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task UpdatesTheSelectedItem()
            {
                Prepare();

                var observer = TestScheduler.CreateObserver<int>();
                ViewModel.SelectedIndex.Subscribe(observer);

                var index = 2;
                await ViewModel.SelectItemAction.Execute(index);

                observer.Messages.Single().Value.Value.Should().Be(index);
            }
        }
    }
}

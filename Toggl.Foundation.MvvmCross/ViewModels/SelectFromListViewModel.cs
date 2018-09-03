using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public abstract class SelectFromListViewModel<T> : MvxViewModel<SelectFromListParameters<T>, T>
    {
        private readonly IMvxNavigationService navigationService;
        private readonly ISubject<int> selectedIndexSubject = new Subject<int>();

        private int selectedIndex;

        public IList<SelectableItem<T>> Items;

        public UIAction BackAction { get; }

        public InputAction<int> SelectItemAction { get; }

        public IObservable<int> SelectedIndex => selectedIndexSubject
            .AsObservable()
            .DistinctUntilChanged();

        public SelectFromListViewModel(IMvxNavigationService navigationService)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            this.navigationService = navigationService;

            BackAction = new UIAction(onBack);
            SelectItemAction = new InputAction<int>(onItemSelected);
        }

        public override void Prepare(SelectFromListParameters<T> parameters)
        {
            Items = parameters.Items;
            selectedIndex = parameters.SelectedIndex;
        }

        private IObservable<Unit> onBack()
        {
            navigationService.Close(this, Items[selectedIndex].Value);
            return Observable.Return(Unit.Default);
        }

        private IObservable<Unit> onItemSelected(int index)
        {
            selectedIndex = index;
            selectedIndexSubject.OnNext(selectedIndex);
            return Observable.Return(Unit.Default);
        }
    }
}

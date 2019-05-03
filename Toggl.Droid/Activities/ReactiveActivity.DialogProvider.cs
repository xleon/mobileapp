using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Services;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel>
    {
        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
            => this.ShowConfirmationDialog(title, message, confirmButtonText, dismissButtonText);

        public IObservable<T> Select<T>(string title, IEnumerable<(string ItemName, T Item)> options, int initialSelectionIndex = 0)
            => this.ShowSelectionDialog(title, options, initialSelectionIndex);

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => this.ShowConfirmationDialog(title, message, buttonTitle, null).Select(_ => Unit.Default);

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
            => this.ShowDestructiveActionConfirmationDialog(type, formatArguments);
    }
}
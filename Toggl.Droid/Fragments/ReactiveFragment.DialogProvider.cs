using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Services;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Fragments
{
    public abstract partial class ReactiveFragment<TViewModel>
    {
        public IObservable<bool> Confirm(string title, string message, string confirmButtonText,
            string dismissButtonText)
        {
            if (Activity == null) 
                return Observable.Return(false);
            
            return Activity.ShowConfirmationDialog(title, message, confirmButtonText, dismissButtonText);  
        } 

        public IObservable<T> Select<T>(string title, IEnumerable<(string ItemName, T Item)> options, int initialSelectionIndex = 0)
            => Activity.ShowSelectionDialog(title, options, initialSelectionIndex);

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
        {
            if (Activity == null)
                return Observable.Return(Unit.Default);
            
            return Activity.ShowConfirmationDialog(title, message, buttonTitle, null).Select(_ => Unit.Default);
        }

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
        {
            if (Activity == null)
                return Observable.Return(false);

            return Activity.ShowDestructiveActionConfirmationDialog(type, formatArguments);  
        } 
    }
}
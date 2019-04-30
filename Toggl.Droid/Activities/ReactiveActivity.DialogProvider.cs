using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.App;
using Toggl.Core.UI.Services;
using Toggl.Droid.Views;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    public abstract partial class ReactiveActivity<TViewModel>
    {
        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
        {
            return Observable.Create<bool>(observer =>
            {
                void showDialogIfActivityIsThere()
                {
                    if (IsFinishing)
                    {
                        observer.CompleteWith(false);
                        return;
                    }

                    var builder = new AlertDialog.Builder(this, Resource.Style.TogglDialog).SetMessage(message)
                        .SetPositiveButton(confirmButtonText, (s, e) => observer.CompleteWith(true));

                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        builder = builder.SetTitle(title);
                    }

                    if (!string.IsNullOrEmpty(dismissButtonText))
                    {
                        builder = builder.SetNegativeButton(dismissButtonText, (s, e) => observer.CompleteWith(false));
                    }

                    var dialog = builder.Create();
                    dialog.CancelEvent += (s, e) => observer.CompleteWith(false);

                    dialog.Show();
                }

                RunOnUiThread(showDialogIfActivityIsThere);

                return Disposable.Empty;
            });
        }

        public IObservable<T> Select<T>(string title, IEnumerable<(string ItemName, T Item)> options, int initialSelectionIndex = 0)
        {
            return Observable.Create<T>(observer =>
            {
                var dialog = new ListSelectionDialog<T>(this, title, options, initialSelectionIndex, observer.CompleteWith);
                RunOnUiThread(dialog.Show);
                return Disposable.Empty;
            });
        }

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => Confirm(title, message, buttonTitle, null).Select(_ => Unit.Default);

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArguments)
        {
            switch (type)
            {
                case ActionType.DiscardNewTimeEntry:
                    return Confirm(null, Core.Resources.DiscardThisTimeEntry, Core.Resources.Discard, Core.Resources.Cancel);
                case ActionType.DiscardEditingChanges:
                    return Confirm(null, Core.Resources.DiscardEditingChanges, Core.Resources.Discard, Core.Resources.ContinueEditing);
                case ActionType.DeleteExistingTimeEntry:
                    return Confirm(null, Core.Resources.DeleteThisTimeEntry, Core.Resources.Delete, Core.Resources.Cancel);
                case ActionType.DeleteMultipleExistingTimeEntries:
                    return Confirm(null, string.Format(Core.Resources.DeleteMultipleTimeEntries, formatArguments), Core.Resources.Delete, Core.Resources.Cancel);
                case ActionType.DiscardFeedback:
                    return Confirm(null, Core.Resources.DiscardMessage, Core.Resources.Discard, Core.Resources.ContinueEditing);
            }

            throw new ArgumentOutOfRangeException(nameof(type));
        }
    }
}
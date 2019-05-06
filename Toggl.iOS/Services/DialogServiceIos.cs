using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreGraphics;
using Toggl.Core;
using Toggl.Core.UI.Services;
using Toggl.iOS.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Services
{
    public sealed class DialogServiceIos : IDialogService
    {
        private readonly ITopViewControllerProvider topViewControllerProvider;

        public DialogServiceIos(ITopViewControllerProvider topViewControllerProvider)
        {
            Ensure.Argument.IsNotNull(topViewControllerProvider, nameof(topViewControllerProvider));

            this.topViewControllerProvider = topViewControllerProvider;
        }

        public IObservable<bool> Confirm(string title, string message, string confirmButtonText, string dismissButtonText)
            => topViewControllerProvider.TopViewController.ShowConfirmDialog(title, message, confirmButtonText, dismissButtonText);

        public IObservable<bool> ConfirmDestructiveAction(ActionType type, params object[] formatArgs)
            => topViewControllerProvider.TopViewController.ShowConfirmDestructiveActionDialog(type, formatArgs);

        public IObservable<Unit> Alert(string title, string message, string buttonTitle)
            => topViewControllerProvider.TopViewController.ShowAlertDialog(title, message, buttonTitle);

        public IObservable<T> Select<T>(string title, IEnumerable<(string ItemName, T Item)> options, int initialSelectionIndex)
            => topViewControllerProvider.TopViewController.ShowSelectDialog(title, options, initialSelectionIndex);
    }
}

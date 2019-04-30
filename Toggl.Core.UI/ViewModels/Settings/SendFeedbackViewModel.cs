using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Interactors;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Services;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Core.UI.Navigation;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SendFeedbackViewModel : ViewModelWithOutput<bool>
    {
        private readonly IDialogService dialogService;
        private readonly IInteractorFactory interactorFactory;
        private readonly INavigationService navigationService;

        // Internal States
        private readonly ISubject<bool> isLoadingSubject = new BehaviorSubject<bool>(false);
        private readonly ISubject<Exception> currentErrorSubject = new BehaviorSubject<Exception>(null);

        // Actions
        public UIAction Close { get; }
        public UIAction DismissError { get; }
        public UIAction Send { get; }

        // Inputs
        public ISubject<string> FeedbackText { get; } = new BehaviorSubject<string>(string.Empty);

        // Outputs
        public IObservable<bool> IsFeedbackEmpty { get; }
        public IObservable<bool> SendEnabled { get; }
        public IObservable<bool> IsLoading { get; }
        public IObservable<Exception> Error { get; }

        private IObservable<bool> isEmptyObservable => FeedbackText.Select(string.IsNullOrEmpty);
        private IObservable<bool> sendingIsEnabledObservable =>
            isEmptyObservable.CombineLatest(
                isLoadingSubject.AsObservable(),
                (isEmpty, isLoading) => !isEmpty && !isLoading);

        public SendFeedbackViewModel(
            INavigationService navigationService,
            IInteractorFactory interactorFactory,
            IDialogService dialogService,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(dialogService, nameof(dialogService));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.dialogService = dialogService;
            this.interactorFactory = interactorFactory;
            this.navigationService = navigationService;

            IsFeedbackEmpty = isEmptyObservable.DistinctUntilChanged().AsDriver(schedulerProvider);
            SendEnabled = sendingIsEnabledObservable.DistinctUntilChanged().AsDriver(schedulerProvider);

            Close = rxActionFactory.FromObservable(cancel);
            DismissError = rxActionFactory.FromAction(dismissError);
            Send = rxActionFactory.FromObservable(sendFeedback, sendingIsEnabledObservable);

            IsLoading = isLoadingSubject.AsDriver(false, schedulerProvider);
            Error = currentErrorSubject.AsDriver(default(Exception), schedulerProvider);
        }

        private void dismissError()
        {
            currentErrorSubject.OnNext(null);
        }

        private IObservable<Unit> cancel()
            => FeedbackText.FirstAsync()
                .Select(string.IsNullOrEmpty)
                .SelectMany(isEmpty => isEmpty
                    ? Observable.Return(true)
                    : this.SelectDialogService(dialogService).ConfirmDestructiveAction(ActionType.DiscardFeedback))
                .DoIf(shouldBeClosed => shouldBeClosed, _ => Finish(false))
                .SelectUnit();

        private IObservable<Unit> sendFeedback()
            => FeedbackText.FirstAsync()
                .Do(_ =>
                {
                    isLoadingSubject.OnNext(true);
                    currentErrorSubject.OnNext(null);
                })
                .SelectMany(text => interactorFactory
                    .SendFeedback(text)
                    .Execute()
                    .Materialize())
                .Do(notification =>
                {
                    switch (notification.Kind)
                    {
                        case NotificationKind.OnError:
                            isLoadingSubject.OnNext(false);
                            currentErrorSubject.OnNext(notification.Exception);
                            break;

                        default:
                            Finish(true);
                            break;
                    }
                })
                .SelectUnit();
    }
}

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Foundation.MvvmCross.Onboarding.MainView;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Sync;
using Toggl.Giskard.Adapters;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.Helper;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Activities
{
    public partial class MainActivity
    {
        private PopupWindow playButtonTooltipPopupWindow;
        private PopupWindow stopButtonTooltipPopupWindow;
        private PopupWindow tapToEditPopup;

        private readonly BehaviorSubject<View> firstTimeEntryViewSubject = new BehaviorSubject<View>(null);
        private EditTimeEntryOnboardingStep editTimeEntryOnboardingStep;
        private IDisposable editTimeEntryOnboardingStepDisposable;

        protected override void OnStop()
        {
            base.OnStop();
            playButtonTooltipPopupWindow.Dismiss();
            stopButtonTooltipPopupWindow.Dismiss();
        }

        private void setupStartTimeEntryOnboardingStep()
        {
            if (playButtonTooltipPopupWindow == null)
            {
                playButtonTooltipPopupWindow = PopupWindowFactory.PopupWindowWithText(
                    this,
                    Resource.Layout.TooltipWithRightArrow,
                    Resource.Id.TooltipText,
                    Resource.String.OnboardingTapToStartTimer);
            }

            new StartTimeEntryOnboardingStep(ViewModel.OnboardingStorage)
                .ManageDismissableTooltip(
                    playButtonTooltipPopupWindow,
                    playButton,
                    (popup, anchor) => popup.LeftVerticallyCenteredOffsetsTo(anchor, dpExtraRightMargin: 8),
                    ViewModel.OnboardingStorage)
                .DisposedBy(DisposeBag);
        }

        private void setupStopTimeEntryOnboardingStep()
        {
            if (stopButtonTooltipPopupWindow == null)
            {
                stopButtonTooltipPopupWindow = PopupWindowFactory.PopupWindowWithText(
                    this,
                    Resource.Layout.TooltipWithRightBottomArrow,
                    Resource.Id.TooltipText,
                    Resource.String.OnboardingTapToStopTimer);
            }

            new StopTimeEntryOnboardingStep(ViewModel.OnboardingStorage, ViewModel.IsTimeEntryRunning)
                .ManageDismissableTooltip(
                    stopButtonTooltipPopupWindow,
                    stopButton,
                    (popup, anchor) => popup.TopRightFrom(anchor, dpExtraBottomMargin: 8),
                    ViewModel.OnboardingStorage)
                .DisposedBy(DisposeBag);
        }

        private void setupTapToEditOnboardingStep()
        {
            editTimeEntryOnboardingStep = new EditTimeEntryOnboardingStep(
                ViewModel.OnboardingStorage, Observable.Return(false));

            var mainRecyclerViewScrollsObservable =
                Observable
                    .FromEventPattern<View.ScrollChangeEventArgs>(e => mainRecyclerView.ScrollChange += e, e => mainRecyclerView.ScrollChange -= e)
                    .Select(_ => Unit.Default);

            var showTapToEditOnboardingStepObservable =
                Observable.CombineLatest(
                    editTimeEntryOnboardingStep.ShouldBeVisible,
                    mainRecyclerViewScrollsObservable,
                    ViewModel.SyncProgressState,
                    (shouldShowStep, unit, syncState) => shouldShowStep && syncState == SyncProgress.Synced);

            showTapToEditOnboardingStepObservable
                .Where(shouldShowStep => shouldShowStep)
                .VoidSubscribe(onFirstTimeEntryViewUpdate)
                .DisposedBy(DisposeBag);

            firstTimeEntryViewSubject
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(updateTapToEditOnboardingStep)
                .DisposedBy(DisposeBag);
        }

        private void onFirstTimeEntryViewUpdate()
        {
            var view = findOldestTimeEntryView();
            firstTimeEntryViewSubject.OnNext(view);
        }

        private View findOldestTimeEntryView()
        {
            var layoutManager = (LinearLayoutManager) mainRecyclerView.GetLayoutManager();
            var adapter = mainRecyclerView.GetAdapter() as MainRecyclerAdapter;
            if (adapter == null)
            {
                return null;
            }

            for (var position = layoutManager.FindLastVisibleItemPosition(); position >= 0; position--)
            {
                var viewType = adapter.GetItemViewType(position);
                if (viewType != MainRecyclerAdapter.ItemViewType)
                {
                    continue;
                }

                var view = layoutManager.FindViewByPosition(position);
                if (view == null || layoutManager.GetItemViewType(view) != MainRecyclerAdapter.ItemViewType)
                    return null;

                var isVisible =
                    layoutManager.IsViewPartiallyVisible(view, true, true)
                    || layoutManager.IsViewPartiallyVisible(view, false, true);

                return isVisible ? view : null;
            }

            return null;
        }

        private void updateTapToEditOnboardingStep(View firstTimeEntry)
        {
            tapToEditPopup?.Dismiss();
            tapToEditPopup = null;

            if (firstTimeEntry == null)
                return;

            updateTapToEditPopupWindow(firstTimeEntry);
        }

        private void updateTapToEditPopupWindow(View firstTimeEntry)
        {
            if (editTimeEntryOnboardingStepDisposable != null)
            {
                editTimeEntryOnboardingStepDisposable.Dispose();
                editTimeEntryOnboardingStepDisposable = null;
            }

            tapToEditPopup?.Dismiss();

            tapToEditPopup = tapToEditPopup
                ?? PopupWindowFactory.PopupWindowWithText(
                    this,
                    Resource.Layout.TooltipWithLeftTopArrow,
                    Resource.Id.TooltipText,
                    Resource.String.OnboardingTapToEdit);

            var storage = ViewModel.OnboardingStorage;

            editTimeEntryOnboardingStepDisposable = new EditTimeEntryOnboardingStep(storage, Observable.Return(false))
                .ManageDismissableTooltip(
                    tapToEditPopup,
                    firstTimeEntry,
                    (window, view) => PopupOffsets.FromDp(16, -4, this),
                    storage);
        }
    }
}

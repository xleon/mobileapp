using Android.Runtime;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class ReportsFragment : ReactiveTabFragment<ReportsViewModel>, IScrollableToStart
    {
        public ReportsFragment(MainTabBarViewModel tabBarViewModel)
            : base(tabBarViewModel)
        {
        }

        public ReportsFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializationFinished()
        {            
            selectWorkspaceFab.Rx().Tap()
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.FormattedTimeRange
                .Subscribe(toolbarCurrentDateRangeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Elements
                .Subscribe(adapter.Rx().Items())
                .DisposedBy(DisposeBag);

            toolbarCurrentDateRangeText.Rx()
                .BindAction(ViewModel.SelectTimeRange)
                .DisposedBy(DisposeBag);
        }

        public void ScrollToStart()
        {
            reportsRecyclerView?.SmoothScrollToPosition(0);
        }
    }
}

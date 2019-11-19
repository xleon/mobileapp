using Android.OS;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class ReportsFragment : ReactiveTabFragment<ReportsViewModel>, IScrollableToStart
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ReportsFragment, container, false);
            InitializeViews(view);
            SetupToolbar(view);
            reportsRecyclerView.AttachMaterialScrollBehaviour(appBarLayout);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            selectWorkspaceFab.Rx().Tap()
                .Subscribe(ViewModel.SelectWorkspace.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.FormattedTimeRange
                .Subscribe(toolbarCurrentDateRangeText.Rx().TextObserver())
                .DisposedBy(DisposeBag);
        }

        public void ScrollToStart()
        {
            reportsRecyclerView?.SmoothScrollToPosition(0);
        }
    }
}

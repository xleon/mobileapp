using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels.DateRangePicker;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.ViewHolders;
using Toggl.Droid.Views;
using Toggl.Shared.Extensions;
using static Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel;

namespace Toggl.Droid.Fragments
{
    public partial class DateRangePickerFragment : ReactiveDialogFragment<DateRangePickerViewModel>
    {
        private DateRangePickerMonthsPagerAdapter adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.DateRangePicker, null);
            InitializeViews(view);
            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            adapter = new DateRangePickerMonthsPagerAdapter(Context);
            monthsPager.Adapter = adapter;

            var shortcutsAdapter = new SimpleAdapter<Shortcut>(
                Resource.Layout.DateRangePickerShortcutCell,
                view => new DateRangePickerShortcutViewHolder(view, Context));

            shortcutsAdapter.ItemTapObservable
                .Select(shortcut => shortcut.DateRangePeriod)
                .Subscribe(ViewModel.SetDateRangePeriod.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Shortcuts
                .Subscribe(shortcutsAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            shortcutsRecyclerView.SetAdapter(shortcutsAdapter);

            weekDaysLabels
                .Indexed()
                .ForEach((view, index) => view.Text = ViewModel.WeekDaysLabels[index]);

            ViewModel.Months
                .Subscribe(adapter.UpdateMonths)
                .DisposedBy(DisposeBag);

            // Make sure this is subscribed after the `adapter.UpdateMonths`
            // subscription so that the last month gets selected correctly
            // (and only the first time the ViewModel.Months emits).
            ViewModel.Months
                .Take(1)
                .Subscribe(selectLastMonth)
                .DisposedBy(DisposeBag);

            var pageSelected = monthsPager.Rx().PageSelected()
                .Select(position => adapter[position]);

            pageSelected
                .Select(month => month.MonthDisplay)
                .Subscribe(monthYearLabel.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            pageSelected
                .Subscribe(recalculateViewPagerHeight)
                .DisposedBy(DisposeBag);

            adapter.DateSelected
                .Subscribe(ViewModel.SelectDate.Execute)
                .DisposedBy(DisposeBag);

            ViewModel.LastSelectedDate
                .Subscribe(scrollToPage)
                .DisposedBy(DisposeBag);

            okButton.Rx()
                .BindAction(ViewModel.Accept)
                .DisposedBy(DisposeBag);

            cancelButton.Rx()
                .BindAction(ViewModel.Cancel)
                .DisposedBy(DisposeBag);
        }

        private void selectLastMonth(ImmutableList<DateRangePickerMonthInfo> months)
        {
            monthsPager.CurrentItem = months.Count - 1;
        }

        private void recalculateViewPagerHeight(DateRangePickerMonthInfo month)
        {
            var rowHeight = DateRangePickerMonthView.RowHeightDp.DpToPixels(Context);
            var pagerSize = month.RowCount * rowHeight;

            var layoutParams = monthsPager.LayoutParameters;
            layoutParams.Height = pagerSize;
            monthsPager.LayoutParameters = layoutParams;
        }

        private void scrollToPage(DateTime? lastSelectedDate)
        {
            if (!lastSelectedDate.HasValue)
                return;

            var index = adapter.FindMonthIndex(lastSelectedDate.Value);
            if (!index.HasValue)
                return;

            monthsPager.CurrentItem = index.Value;
        }

        public override void OnResume()
        {
            base.OnResume();

            var (widthPixels, _, _) = Activity.GetMetrics(Context);
            var width = widthPixels - 24.DpToPixels(Context);
            Dialog.Window.SetLayout(width, ViewGroup.LayoutParams.WrapContent);

            Dialog.Window.DecorView.SetBackgroundColor(backgroundColor);
        }
    }
}

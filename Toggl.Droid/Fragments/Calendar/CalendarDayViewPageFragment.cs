using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.Views;
using Toggl.Droid.Views.Calendar;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;

namespace Toggl.Droid.Fragments.Calendar
{
    public partial class CalendarDayViewPageFragment : Fragment, IView
    {
        private readonly TimeSpan defaultTimeEntryDurationForCreation = TimeSpan.FromMinutes(30);
        private CompositeDisposable DisposeBag = new CompositeDisposable();
        private CalendarDayView calendarDayView;

        public CalendarDayViewModel ViewModel { get; set; }
        public BehaviorRelay<int> CurrentPageRelay { get; set; }
        public BehaviorRelay<int> ScrollOffsetRelay { get; set; }
        public IObservable<bool> ScrollToStartSign { get; set; }
            
        public int PageNumber { get; set; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            calendarDayView = new CalendarDayView(Context);
            return calendarDayView;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            if (ViewModel == null) return;
            
            ViewModel?.AttachView(this);
            calendarDayView.SetCurrentDate(ViewModel.Date);
            calendarDayView.SetOffset(ScrollOffsetRelay?.Value ?? 0);
            calendarDayView.UpdateItems(ViewModel.CalendarItems);
            
            ViewModel.CalendarItems.CollectionChange
                .Subscribe(_ => calendarDayView.UpdateItems(ViewModel.CalendarItems))
                .DisposedBy(DisposeBag);

            calendarDayView.CalendarItemTappedObservable
                .Subscribe(ViewModel.OnItemTapped.Inputs)
                .DisposedBy(DisposeBag);

            calendarDayView.EmptySpansTouchedObservable
                .Select(emptySpan => (emptySpan, defaultTimeEntryDurationForCreation))
                .Subscribe(ViewModel.OnDurationSelected.Inputs)
                .DisposedBy(DisposeBag);

            calendarDayView.EditCalendarItem
                .Subscribe(ViewModel.OnTimeEntryEdited.Inputs)
                .DisposedBy(DisposeBag);

            ScrollToStartSign?
                .Subscribe(scrollToStart)
                .DisposedBy(DisposeBag);

            calendarDayView.ScrollOffsetObservable
                .Subscribe(updateScrollOffsetIfCurrentPage)
                .DisposedBy(DisposeBag);
        }

        private void updateScrollOffsetIfCurrentPage(int scrollOffset)
        {
            if (PageNumber != CurrentPageRelay?.Value) 
                return;
            
            ScrollOffsetRelay?.Accept(scrollOffset);
        }

        private void scrollToStart(bool shouldSmoothScroll)
        {
            if (PageNumber != CurrentPageRelay?.Value)
                return;
            
            calendarDayView?.ScrollToCurrentHour(shouldSmoothScroll);
        }

        public override void OnDestroyView()
        {
            ViewModel?.DetachView();
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
            base.OnDestroyView();
        }
    }
}
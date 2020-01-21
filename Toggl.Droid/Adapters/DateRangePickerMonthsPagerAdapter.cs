using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.ViewPager.Widget;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Linq;
using Toggl.Core.UI.ViewModels.DateRangePicker;
using Toggl.Droid.Views;
using Toggl.Shared.Extensions;
using Toggl.Shared.Extensions.Reactive;
using Object = Java.Lang.Object;

namespace Toggl.Droid.Fragments
{
    public class DateRangePickerMonthsPagerAdapter : PagerAdapter
    {
        private readonly Context context;

        private ImmutableList<DateRangePickerMonthInfo> months = ImmutableList<DateRangePickerMonthInfo>.Empty;

        private BehaviorRelay<DateTime?> dateSelectedRelay = new BehaviorRelay<DateTime?>(null);

        public IObservable<DateTime> DateSelected { get; private set; }

        public DateRangePickerMonthsPagerAdapter(Context context)
        {
            this.context = context;

            DateSelected = dateSelectedRelay
                .WhereNotNull()
                .AsObservable();
        }

        public DateRangePickerMonthsPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public int? FindMonthIndex(DateTime value)
        {
            var index = months.FindLastIndex(m => m.Month == value.Month && m.Year == value.Year);

            return index >= 0 ? index : (int?)null;
        }

        public DateRangePickerMonthInfo this[int index]
            => months[index];

        public override Object InstantiateItem(ViewGroup container, int position)
        {
            var inflater = LayoutInflater.FromContext(context);
            var inflatedView = inflater.Inflate(Resource.Layout.DateRangePickerMonthView, container, false);

            var monthView = inflatedView as DateRangePickerMonthView;
            monthView.Setup(months[position], dateSelectedRelay);

            container.AddView(inflatedView);
            return inflatedView;
        }

        public override void DestroyItem(ViewGroup container, int position, Object obj)
        {
            container.RemoveView(obj as View);
        }

        public override int GetItemPosition(Object @object)
            => PositionNone;

        public void UpdateMonths(ImmutableList<DateRangePickerMonthInfo> months)
        {
            this.months = months;
            NotifyDataSetChanged();
        }

        public override int Count
            => months.Count;

        public override bool IsViewFromObject(View view, Object obj)
            => view == obj;
    }
}

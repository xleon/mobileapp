using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using AndroidX.ViewPager.Widget;
using System.Collections.Generic;
using System.Linq;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Fragments
{
    public partial class DateRangePickerFragment
    {
        private Color backgroundColor;

        private TextView okButton;
        private TextView cancelButton;
        private TextView monthYearLabel;
        private ViewPager monthsPager;
        private RecyclerView shortcutsRecyclerView;
        private List<TextView> weekDaysLabels;

        protected override void InitializeViews(View view)
        {
            backgroundColor = Context.SafeGetColor(Resource.Color.dateRangePickerBackground);

            monthYearLabel = view.FindViewById<TextView>(Resource.Id.MonthYearLabel);

            weekDaysLabels = view.FindViewById<ViewGroup>(Resource.Id.MonthNamesContainer)
                .GetChildren<TextView>()
                .ToList();

            okButton = view.FindViewById<TextView>(Resource.Id.OkButton);
            okButton.Text = Shared.Resources.Ok;

            cancelButton = view.FindViewById<TextView>(Resource.Id.CancelButton);
            cancelButton.Text = Shared.Resources.Cancel;

            monthsPager = view.FindViewById<ViewPager>(Resource.Id.MonthsPager);

            shortcutsRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.Shortcuts);
            shortcutsRecyclerView.SetLayoutManager(new LinearLayoutManager(Context, LinearLayoutManager.Horizontal, false));
        }
    }
}

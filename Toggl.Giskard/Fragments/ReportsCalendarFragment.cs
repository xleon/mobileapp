using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Giskard.Extensions;
using Toggl.Multivac.Extensions;
using Toggl.Foundation.MvvmCross.ViewModels;
using System.Linq;

namespace Toggl.Giskard.Fragments
{
    [MvxFragmentPresentation(typeof(ReportsViewModel), Resource.Id.ReportsCalendarContainer, AddToBackStack = false)]
    public sealed class ReportsCalendarFragment : MvxFragment<ReportsCalendarViewModel>
    {

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.ReportsCalendarFragment, null);

            var calendarHeader = view.FindViewById<LinearLayout>(Resource.Id.ReportsCalendarFragmentHeader);

            calendarHeader
                .GetChildren<TextView>()
                .Indexed()
                .ForEach((TextView textView, int index) 
                    => textView.Text = ViewModel.DayHeaderFor(index));

            return view;
        }
    }
}

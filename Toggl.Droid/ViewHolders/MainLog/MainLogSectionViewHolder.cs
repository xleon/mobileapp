using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;

namespace Toggl.Droid.ViewHolders.MainLog
{
    public class MainLogSectionViewHolder : BaseRecyclerViewHolder<MainLogItemViewModel>
    {
            private TextView mainLogHeaderTitle;
            private TextView mainLogHeaderDuration;

            public MainLogSectionViewHolder(View itemView) : base(itemView)
            {
            }

            public MainLogSectionViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
            {
            }

            protected override void InitializeViews()
            {
                mainLogHeaderTitle = ItemView.FindViewById<TextView>(Resource.Id.MainLogHeaderTitle);
                mainLogHeaderDuration = ItemView.FindViewById<TextView>(Resource.Id.MainLogHeaderDuration);
            }

            protected override void UpdateView()
            {
                mainLogHeaderTitle.Text = (Item as DaySummaryViewModel).Title;
                mainLogHeaderDuration.Text = (Item as DaySummaryViewModel).TotalTrackedTime;
            }
        }

}

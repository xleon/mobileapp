using System;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Transformations;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.TimeEntriesLog;
using Toggl.Droid.ViewHelpers;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public class MainLogSectionViewHolder : BaseRecyclerViewHolder<DaySummaryViewModel>
    {
        private TextView mainLogHeaderTitle;
        private TextView mainLogHeaderDuration;

        public DateTimeOffset Now { private get; set; }

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
            mainLogHeaderTitle.Text = Item.Title;
            mainLogHeaderDuration.Text = Item.TotalTrackedTime;
        }
    }
}

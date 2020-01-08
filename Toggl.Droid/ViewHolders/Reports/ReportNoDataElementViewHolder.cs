using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportNoDataElementViewHolder : ReportElementViewHolder<ReportNoDataElement>
    {
        private TextView title;
        private TextView message;

        public ReportNoDataElementViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportNoDataElementViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            title = ItemView.FindViewById<TextView>(Resource.Id.Title);
            message = ItemView.FindViewById<TextView>(Resource.Id.Message);

            title.Text = Resources.ReportsEmptyStateTitle;
            message.Text = Resources.ReportsEmptyStateDescription;
        }

        protected override void UpdateView() { }
    }
}

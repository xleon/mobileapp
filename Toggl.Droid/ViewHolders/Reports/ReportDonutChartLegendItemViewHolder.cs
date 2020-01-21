using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.Transformations;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Views;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportDonutChartLegendItemViewHolder : ReportElementViewHolder<ReportProjectsDonutChartLegendItemElement>
    {
        private TextView projectName;
        private TextView clientName;
        private TextView duration;
        private TextView percentage;

        public ReportDonutChartLegendItemViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportDonutChartLegendItemViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            projectName = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemProjectName);
            clientName = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemClientName);
            duration = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemDuration);
            percentage = ItemView.FindViewById<TextView>(Resource.Id.ReportsFragmentItemPercentage);
        }

        protected override void UpdateView()
        {
            projectName.Text = Item.Name;
            projectName.SetTextColor(Color.ParseColor(Item.Color));
            clientName.Text = Item.Client;
            duration.Text = Item.Value;
            percentage.Text = $"{Item.Percentage:0.00}%";
        }
    }
}

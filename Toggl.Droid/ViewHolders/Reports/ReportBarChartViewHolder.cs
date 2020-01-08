using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Views;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportBarChartViewHolder : ReportElementViewHolder<ReportBarChartElement>
    {
        private BarChartView barChart;

        public ReportBarChartViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportBarChartViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            barChart = ItemView.FindViewById<BarChartView>(Resource.Id.BarChartView);
            var billableLabel = ItemView.FindViewById<TextView>(Resource.Id.BillableText);
            var nonBillableLabel = ItemView.FindViewById<TextView>(Resource.Id.NonBillableText);
            var chartTitle = ItemView.FindViewById<TextView>(Resource.Id.ClockedHours);

            billableLabel.Text = Resources.Billable;
            nonBillableLabel.Text = Resources.NonBillable;
            chartTitle.Text = Resources.ClockedHours;
        }

        protected override void UpdateView()
        {
            barChart.updateBars(Item.Bars, Item.MaxBarValue, Item.XLabels, Item.YLabels);
        }
    }
}
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Views;

namespace Toggl.Droid.ViewHolders.Reports
{
    public class ReportDonutChartDonutViewHolder : ReportElementViewHolder<ReportDonutChartDonutElement>
    {
        private DonutChartView donutChartView;

        public ReportDonutChartDonutViewHolder(View itemView) : base(itemView)
        {
        }

        public ReportDonutChartDonutViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            donutChartView = ItemView.FindViewById<DonutChartView>(Resource.Id.DonutView);
        }

        protected override void UpdateView()
        {
            donutChartView.Update(Item);
        }
    }
}

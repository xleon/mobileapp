using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Droid.Adapters;
using Toggl.Droid.ViewHolders;
using Toggl.Droid.ViewHolders.Reports;

namespace Toggl.Droid.Fragments
{
    public class ReportsAdapter : BaseRecyclerAdapter<IReportElement>
    {
        private enum ViewType
        {
            WorkspaceName,
            Summary,
            BarChart,
            Donut,
            DonutLegendItem,
            NoData,
            Error
        }

        public ReportsAdapter()
        {
        }

        public ReportsAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override int GetItemViewType(int position)
        {
            switch (Items[position])
            {
                case ReportWorkspaceNameElement _:
                    return (int)ViewType.WorkspaceName;

                case ReportSummaryElement _:
                    return (int)ViewType.Summary;

                case ReportBarChartElement _:
                    return (int)ViewType.BarChart;

                case ReportDonutChartDonutElement _:
                    return (int)ViewType.Donut;

                case ReportDonutChartLegendItemElement _:
                    return (int)ViewType.DonutLegendItem;

                case ReportNoDataElement _:
                    return (int)ViewType.NoData;

                case ReportErrorElement _:
                    return (int)ViewType.Error;

                default:
                    throw new InvalidOperationException("Invalid Report Segment View Type.");
            }
        }

        protected override BaseRecyclerViewHolder<IReportElement> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            switch ((ViewType)viewType)
            {
                case ViewType.WorkspaceName:
                    var workpaceNameCell = inflater.Inflate(Resource.Layout.ReportWorkspaceNameElement, parent, false);
                    return new ReportWorkspaceNameViewHolder(workpaceNameCell);

                case ViewType.Summary:
                    var summaryCell = inflater.Inflate(Resource.Layout.ReportSummaryElement, parent, false);
                    return new ReportSummaryViewHolder(summaryCell);

                case ViewType.BarChart:
                    var barChartCell = inflater.Inflate(Resource.Layout.ReportEmptyElement, parent, false);
                    return new ReportEmptyElementViewHolder(barChartCell);

                case ViewType.Donut:
                    var donutCell = inflater.Inflate(Resource.Layout.ReportEmptyElement, parent, false);
                    return new ReportEmptyElementViewHolder(donutCell);

                case ViewType.DonutLegendItem:
                    var donutLegendItemCell = inflater.Inflate(Resource.Layout.ReportEmptyElement, parent, false);
                    return new ReportEmptyElementViewHolder(donutLegendItemCell);

                case ViewType.NoData:
                    var noDataCell = inflater.Inflate(Resource.Layout.ReportEmptyElement, parent, false);
                    return new ReportEmptyElementViewHolder(noDataCell);

                case ViewType.Error:
                    var errorCell = inflater.Inflate(Resource.Layout.ReportEmptyElement, parent, false);
                    return new ReportEmptyElementViewHolder(errorCell);

                default:
                    throw new InvalidOperationException("Can't create a viewholder for the given ViewType.");
            }
        }
    }
}

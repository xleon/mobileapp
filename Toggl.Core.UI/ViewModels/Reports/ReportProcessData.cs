using System;
using System.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Models.Interfaces;
using DateRangeSelectionResult = Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel.DateRangeSelectionResult;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public struct ReportProcessData
    {
        public ReportFilter Filter { get; private set; }
        public DateRangeSelectionSource SelectionSource { get; private set; }

        public ReportProcessData(ReportFilter filter, DateRangeSelectionSource selectionSource)
        {
            Filter = filter;
            SelectionSource = selectionSource;
        }

        public static ReportProcessData Create(IThreadSafeWorkspace workspace, DateRangeSelectionResult dateRangeSelectionResult)
            => new ReportProcessData(
                ReportFilter.Create(workspace, dateRangeSelectionResult.SelectedRange.Value.ToLocalInstantaneousTimeRange()),
                dateRangeSelectionResult.Source);
    }
}

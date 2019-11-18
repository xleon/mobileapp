using System;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportErrorElement : IReportElement
    {
        public string Message { get; private set; }

        public ReportErrorElement(Exception ex)
        {
            // TODO: Use the exception to prepare all the data needed to display it in a friendly way to the user
            Message = ex.Message;
        }
    }
}

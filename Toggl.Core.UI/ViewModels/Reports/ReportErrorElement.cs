using System;
using Toggl.Networking.Exceptions;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportErrorElement : IReportElement
    {
        enum ErrorType
        {
            ConnectionError,
            DataError
        }

        private ErrorType errorType;

        public string Message { get; private set; }

        public ReportErrorElement(Exception ex)
        {
            // TODO: Use the exception to prepare all the data needed to display it in a friendly way to the user
            switch (ex)
            {
                case OfflineException _:
                    Message = "Your internet connection ded.";
                    errorType = ErrorType.ConnectionError;
                    break;

                default:
                    Message = "Ooops! Something's wrong. Or is it?";
                    errorType = ErrorType.DataError;
                    break;
            }
        }

        public bool Equals(IReportElement other)
            => other is ReportErrorElement errorElement
            && errorElement.errorType == errorType;
    }
}

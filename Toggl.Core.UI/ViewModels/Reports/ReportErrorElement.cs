using System;
using Toggl.Networking.Exceptions;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public sealed class ReportErrorElement : IReportElement
    {
        public enum ErrorType
        {
            ConnectionError,
            DataError
        }

        public ErrorType Type { get; private set; }
        public string Message { get; private set; }

        public ReportErrorElement(Exception ex)
        {
            // TODO: Use the exception to prepare all the data needed to display it in a friendly way to the user
            switch (ex)
            {
                case OfflineException _:
                    Message = Resources.ReportErrorOffline;
                    Type = ErrorType.ConnectionError;
                    break;

                default:
                    Message = Resources.ReportErrorGeneric;
                    Type = ErrorType.DataError;
                    break;
            }
        }

        public bool Equals(IReportElement other)
            => GetType() == other.GetType()
            && other is ReportErrorElement errorElement
            && errorElement.Type == Type;
    }
}

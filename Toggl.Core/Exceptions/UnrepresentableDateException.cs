using System;
namespace Toggl.Core.Exceptions
{
    public sealed class UnrepresentableDateException : ArgumentOutOfRangeException
    {
        public UnrepresentableDateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

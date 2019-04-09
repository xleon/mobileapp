using System;
using Toggl.Shared;

namespace Toggl.Foundation.Extensions
{
    public static class ExceptionExtensions
    {
        public static bool IsAnonymized(this Exception exception)
            => Attribute.IsDefined(exception.GetType(), typeof(IsAnonymizedAttribute));
    }
}

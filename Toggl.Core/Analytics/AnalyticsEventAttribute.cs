using System;

namespace Toggl.Core.Analytics
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AnalyticsEventAttribute : Attribute
    {
        public string[] ParameterNames { get; }

        public AnalyticsEventAttribute(params string[] parameterNames)
        {
            ParameterNames = parameterNames;
        }
    }
}

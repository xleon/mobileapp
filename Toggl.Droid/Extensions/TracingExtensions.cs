using System;
using Toggl.Core.Diagnostics;

namespace Toggl.Droid.Extensions
{
    public static class TracingExtensions
    {
        [ThreadStatic]
        private static readonly Random samplingRandom = new Random();

        public static IStopwatch MaybeCreateStopwatch(this IStopwatchProvider stopwatchProvider, MeasuredOperation operation, float probability)
        {
            var samplingFactor = samplingRandom.NextDouble();
            if (samplingFactor <= probability)
            {
                return stopwatchProvider.Create(operation);
            }

            return null;
        }
    }
}

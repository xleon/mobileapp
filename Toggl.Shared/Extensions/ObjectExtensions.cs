using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;

namespace Toggl.Shared.Extensions
{
    public static class ObjectExtensions
    {
        private static ConcurrentDictionary<object, TimingData> timers = new ConcurrentDictionary<object, TimingData>();

        public static void startTimer(this object obj, string tag = "")
        {
            var timingData = timers.GetOrDefault(obj, new TimingData());
            timingData.addTimer(tag);

            timers[obj] = timingData;
        }

        public static string logTime(this object obj)
        {
            var timingData = timers[obj];

            var timer = timingData.GetCurrentTimer();
            var tag = timingData.GetCurrentTag();

            return $"[TIMER] {timer.ElapsedMilliseconds}ms {tag}#{obj.GetType().GetFriendlyName()}#{obj.GetHashCode()}";
        }

        public static void debugLog(this object obj, string message)
        {
            Debug.WriteLine($"[DEBUG] {message}");
        }

        private class TimingData
        {
            public Stack<Stopwatch> Stopwatches { get; } = new Stack<Stopwatch>();
            public Stack<string> Tags { get; } = new Stack<string>();

            public void addTimer(string tag)
            {
                Tags.Push(tag);
                Stopwatches.Push(Stopwatch.StartNew());
            }

            public Stopwatch GetCurrentTimer() => Stopwatches.Pop();
            public string GetCurrentTag() => $"{Tags.Pop()}#depth({Tags.Count})";
        }
    }


}

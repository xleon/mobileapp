using System;

namespace Toggl.Foundation.Sync
{
    internal sealed class RetryDelayService : IRetryDelayService
    {
        private readonly Random random;

        private double? lastDelay;

        private const double defaultFastDelay = 10;

        private const double defaultSlowDelay = 60;

        private double randomFastFactor => getRandomNumberBetween(1, 1.5);

        private double randomSlowFactor => getRandomNumberBetween(1.5, 2);

        public RetryDelayService(Random random)
        {
            this.random = random;
            Reset();
        }

        public TimeSpan NextSlowDelay()
            => nextDelay(randomSlowFactor, defaultSlowDelay);

        public TimeSpan NextFastDelay()
            => nextDelay(randomFastFactor, defaultFastDelay);

        public void Reset() => lastDelay = null;

        private TimeSpan nextDelay(double factor, double defaultDelay)
        {
            lastDelay = !lastDelay.HasValue
                ? defaultDelay
                : Math.Max(Math.Min(lastDelay.Value * factor, TimeSpan.MaxValue.TotalSeconds), 0);

            return TimeSpan.FromSeconds(lastDelay.Value);
        }

        private double getRandomNumberBetween(double min, double max)
            => random.NextDouble() * (max - min) + min;
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Toggl.Shared.Extensions
{
    public static class RandomExtensions
    {
        private static int seed = Environment.TickCount;

        private static readonly ThreadLocal<Random> random =
            new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref seed)));

        [return: MaybeNull]
        public static T Sample<T>(this IList<T> items)
        {
            if (items.Count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            return items[random.Value!.Next(items.Count)];
        }
    }
}

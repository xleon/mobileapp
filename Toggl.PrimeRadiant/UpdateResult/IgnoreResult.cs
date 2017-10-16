﻿namespace Toggl.PrimeRadiant
{
    public sealed class IgnoreResult<T> : IConflictResolutionResult<T>
    {
        public long Id { get; }

        public IgnoreResult(long id)
        {
            Id = id;
        }
    }
}

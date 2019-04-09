using System;
using Toggl.Shared.Models;

namespace Toggl.PrimeRadiant
{
    public interface ISinceParameterRepository
    {
        DateTimeOffset? Get<T>()
            where T : ILastChangedDatable;

        void Set<T>(DateTimeOffset? since);

        bool Supports<T>();

        void Reset();
    }
}

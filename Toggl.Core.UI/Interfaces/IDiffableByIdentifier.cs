using System;

namespace Toggl.Core.MvvmCross.Interfaces
{
    public interface IDiffableByIdentifier<T> : IEquatable<T>
    {
        long Identifier { get; }
    }
}

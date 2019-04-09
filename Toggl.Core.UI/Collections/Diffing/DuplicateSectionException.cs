using System;

namespace Toggl.Core.MvvmCross.Collections.Diffing
{
    public sealed class DuplicateSectionException<TKey> : Exception
    {
        public TKey DuplicatedIdentity { get; }

        public DuplicateSectionException(TKey identity)
        {
            DuplicatedIdentity = identity;
        }
    }
}

using System;

namespace Toggl.Foundation.MvvmCross.Collections.Diffing
{
    public sealed class DuplicateItemException : Exception
    {
        public long DuplicatedIdentity { get; }

        public DuplicateItemException(long identity)
        {
            DuplicatedIdentity = identity;
        }
    }
}

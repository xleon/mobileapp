using System;

namespace Toggl.Foundation.MvvmCross.Collections.Diffing
{
    public sealed class DuplicateSectionException : Exception
    {
        public long DuplicatedIdentity { get; }

        public DuplicateSectionException(long identity)
        {
            DuplicatedIdentity = identity;
        }
    }
}

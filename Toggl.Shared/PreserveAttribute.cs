using System;

namespace Toggl.Multivac
{
    public sealed class PreserveAttribute : Attribute
    {
        public bool AllMembers;
        public bool Conditional;
    }
}

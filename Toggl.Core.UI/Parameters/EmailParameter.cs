using System;
using Toggl.Shared;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class EmailParameter
    {
        public Email Email { get; set; }

        public static EmailParameter With(Email email)
            => new EmailParameter { Email = email };
    }
}

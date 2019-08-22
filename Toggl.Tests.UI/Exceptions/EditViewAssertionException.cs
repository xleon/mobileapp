using System;

namespace Toggl.Tests.UI.Exceptions
{
    public sealed class EditViewAssertionException : Exception
    {
        public EditViewAssertionException(string message) : base(message)
        {
        }
    }
}

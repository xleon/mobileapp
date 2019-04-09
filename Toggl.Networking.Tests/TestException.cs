using System;

namespace Toggl.Networking.Tests
{
    sealed class TestException : Exception
    {
        public TestException(string message)
            : base(message)
        {
        }
    }
}

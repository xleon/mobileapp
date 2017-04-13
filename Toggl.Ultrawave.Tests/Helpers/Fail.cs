using System;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Toggl.Ultrawave.Tests
{
    internal static class Fail
    {
        public static Action<T> On<T>() => _ => Assert.True(false);
    }
}

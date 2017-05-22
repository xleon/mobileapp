using System;
using FluentAssertions;
using Xunit;

namespace Toggl.Multivac.Tests
{
    public class EnsureTests
    {
        public class TheArgumentIsNotNullMethod
        {
            [Fact]
            public void ThrowsWhenTheArgumentIsNull()
            {
                const string argumentName = "argument";

                Action whenTheCalledArgumentIsNull =
                    () => Ensure.ArgumentIsNotNull<string>(null, argumentName);

                whenTheCalledArgumentIsNull
                    .ShouldThrow<ArgumentException>()
                    .WithMessage("Value cannot be null.\nParameter name: argument");
            }

            [Fact]
            public void DoesNotThrowWhenTheArgumentIsNotNull()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.ArgumentIsNotNull("something", "argument");

                whenTheCalledArgumentIsNull.ShouldNotThrow();
            }

            [Fact]
            public void WorksForValueTypes()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.ArgumentIsNotNull(0, "argument");

                whenTheCalledArgumentIsNull.ShouldNotThrow();
            }
        }

        public class TheUriIsAbsoluteMethod
        {
            [Fact]
            public void ThrowsWhenTheUriIsNotAbsolute()
            {
                const string argumentName = "argument";

                Action whenTheCalledArgumentIsNull =
                    () => Ensure.UriIsAbsolute(new Uri("/something", UriKind.Relative), argumentName);
                
                whenTheCalledArgumentIsNull
                    .ShouldThrow<ArgumentException>()
                    .WithMessage("Uri must be absolute.\nParameter name: argument");
            }

            [Fact]
            public void DoesNotThrowWhenUriIsAbsolute()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.UriIsAbsolute(new Uri("http://www.toggl.com", UriKind.Absolute), "argument");

                whenTheCalledArgumentIsNull.ShouldNotThrow();
            }

            [Fact]
            public void ThrowsIfTheUriIsNull()
            {
                Action whenTheCalledArgumentIsNull =
                    () => Ensure.UriIsAbsolute(null, "argument");

                whenTheCalledArgumentIsNull
                    .ShouldThrow<ArgumentException>()
                    .WithMessage("Value cannot be null.\nParameter name: argument"); ;
            }
        }
    }
}
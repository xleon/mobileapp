using System;
using FluentAssertions;
using Xunit;

namespace Toggl.Multivac.Tests
{
    public class EmailTests
    {
        public class TheFromStringMethod
        {
            [Fact]
            public void ReturnsInvalidEmailIfTheProvidedEmailIsNotValid()
            {
                var email = Email.FromString("foo@");

                email.Should().Be(Email.Invalid);
            }

            [Fact]
            public void ReturnsValidEmailIfTheProvidedEmailIsValid()
            {
                var email = Email.FromString("susancalvin@psychohistorian.museum");

                email.Should().NotBe(Email.Invalid);
            }
        }

        public class TheIsValidProperty
        {
            [Fact]
            public void ReturnsFalseIfTheEmailWasCreatedUsingTheDefaultConstructor()
            {
                var email = new Email();

                email.IsValid.Should().BeFalse();
            }

            [Fact]
            public void ReturnsFalseIfTheEmailIsTheSameInstanceAsTheInvalidStaticProperty()
            {
                var email = Email.Invalid;

                email.IsValid.Should().BeFalse();
            }

            [Fact]
            public void ReturnsFalseForAnEmailCreatedWithAnInvalidEmailString()
            {
                var email = Email.FromString("foo@");

                email.IsValid.Should().BeFalse();
            }

            [Fact]
            public void ReturnsTrueForAProperlyInitializedValidEmail()
            {
                var email = Email.FromString("susancalvin@psychohistorian.museum");

                email.IsValid.Should().BeTrue();
            }
        }

        public class TheToStringMethod
        {
            [Fact]
            public void ReturnsNullWhenTheEmailIsInvalid()
            {
                var email = Email.Invalid;

                email.ToString().Should().BeNull();
            }

            [Fact]
            public void ReturnsTheStringUsedForConstructionWhenTheEmailIsValid()
            {
                var emailString = "susancalvin@psychohistorian.museum";
                var email = Email.FromString(emailString);

                email.ToString().Should().Be(emailString);
            }
        }
    }
}

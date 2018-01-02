using FluentAssertions;
using Xunit;

namespace Toggl.Multivac.Tests
{
    public sealed class EmailTests
    {
        public sealed class TheFromStringMethod
        {
            [Fact, LogIfTooSlow]
            public void ReturnsInvalidEmailIfTheProvidedEmailIsNotValid()
            {
                var email = Email.FromString("foo@");

                email.Should().Be(Email.Invalid);
            }

            [Fact, LogIfTooSlow]
            public void ReturnsValidEmailIfTheProvidedEmailIsValid()
            {
                var email = Email.FromString("susancalvin@psychohistorian.museum");

                email.Should().NotBe(Email.Invalid);
            }
        }

        public sealed class TheIsValidProperty
        {
            [Fact, LogIfTooSlow]
            public void ReturnsFalseIfTheEmailWasCreatedUsingTheDefaultConstructor()
            {
                var email = new Email();

                email.IsValid.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseIfTheEmailIsTheSameInstanceAsTheInvalidStaticProperty()
            {
                var email = Email.Invalid;

                email.IsValid.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsFalseForAnEmailCreatedWithAnInvalidEmailString()
            {
                var email = Email.FromString("foo@");

                email.IsValid.Should().BeFalse();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTrueForAProperlyInitializedValidEmail()
            {
                var email = Email.FromString("susancalvin@psychohistorian.museum");

                email.IsValid.Should().BeTrue();
            }
        }

        public sealed class TheToStringMethod
        {
            [Fact, LogIfTooSlow]
            public void ReturnsNullWhenTheEmailIsInvalid()
            {
                var email = Email.Invalid;

                email.ToString().Should().BeNull();
            }

            [Fact, LogIfTooSlow]
            public void ReturnsTheStringUsedForConstructionWhenTheEmailIsValid()
            {
                var emailString = "susancalvin@psychohistorian.museum";
                var email = Email.FromString(emailString);

                email.ToString().Should().Be(emailString);
            }
        }
    }
}

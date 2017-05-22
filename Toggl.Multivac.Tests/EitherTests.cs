using System;
using FluentAssertions;
using Xunit;

namespace Toggl.Multivac.Tests
{
    public class EitherTests
    {
        public class TheLeftProperty
        {
            [Fact]
            public void CannotBeAccessedInAnObjectConstructedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                Action accessingLeftInARightObject =
                    () => { var a = either.Left; };

                accessingLeftInARightObject
                    .ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void CanBeAccessedInAnObjectConstructedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                Action accessingLeftInALeftObject =
                    () => { var a = either.Left; };

                accessingLeftInALeftObject.ShouldNotThrow();
            }
        }

        public class TheIsLeftProperty
        {
            [Fact]
            public void ShouldBeTrueForAnObjectCreatedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                either.IsLeft.Should().BeTrue();
            }

            [Fact]
            public void ShouldBeTrueForAnObjectCreatedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                either.IsLeft.Should().BeFalse();
            }
        }

        public class TheRightProperty
        {
            [Fact]
            public void CannotBeAccessedInAnObjectConstructedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");

                Action accessingRightInALeftObject =
                    () => { var a = either.Right; };

                accessingRightInALeftObject
                    .ShouldThrow<InvalidOperationException>();
            }

            [Fact]
            public void CanBeAccessedInAnObjectConstructedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                Action accessingRightInARightObject =
                    () => { var a = either.Right; };

                accessingRightInARightObject.ShouldNotThrow();
            }
        }

        public class TheIsRightProperty
        {
            [Fact]
            public void ShouldBeTrueForAnObjectCreatedWithRight()
            {
                var either = Either<string, bool>.WithRight(true);

                either.IsRight.Should().BeTrue();
            }
            
            [Fact]
            public void ShouldBeFalseForAnObjectCreatedWithLeft()
            {
                var either = Either<string, bool>.WithLeft("");
                
                either.IsRight.Should().BeFalse();
            }
        }

    }
}
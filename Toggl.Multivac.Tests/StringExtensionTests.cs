using System.Linq;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Toggl.Multivac.Extensions;

namespace Toggl.Multivac.Tests
{
    public sealed class StringExtensionTests
    {
        public sealed class TheContainsIgnoringCaseMethod
        {
            [Property]
            public void MatchesSubstringsEvenIfTheyAreAllSearchedInAllCaps(string testString)
            {
                if (string.IsNullOrEmpty(testString)) return;

                var length = testString.Length;
                var startIndex = Gen.Choose(0, length - 1).Sample(1, 1).Single();
                var substring = testString.Substring(startIndex, length - startIndex);

                testString.ContainsIgnoringCase(substring.ToUpper()).Should().BeTrue();
            }

            [Property]
            public void MatchesSubstringsEvenIfTheyAreAllSearchedInAllLowerCase(string testString)
            {
                if (string.IsNullOrEmpty(testString)) return;

                var length = testString.Length;
                var startIndex = Gen.Choose(0, length - 1).Sample(1, 1).Single();
                var substring = testString.Substring(startIndex, length - startIndex);

                testString.ContainsIgnoringCase(substring.ToLower()).Should().BeTrue();
            }
        }
    }
}

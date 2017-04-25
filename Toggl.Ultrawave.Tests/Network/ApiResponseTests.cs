using System;
using FluentAssertions;
using Toggl.Ultrawave.Network;
using Xunit;

namespace Toggl.Ultrawave.Tests
{
    public class ApiResponseTests
    {
        public class TheConstructors
        {
            [Fact]
            public void CreateSuccessfulResponse()
            {
                var responseText = "Test string";
                var response = new ApiResponse<string>(responseText);

                response.Success.Should().BeTrue();

                Action<string> left = arg => arg.Should().Be(responseText);
                Action<ErrorMessage> right = arg => Fail.On<string>();

                response.Data.Match(left, right);
                
            }

            [Fact]
            public void CreateUnsuccessfulResponse()
            {
                var errorMessage = new ErrorMessage();
                var response = new ApiResponse<string>(errorMessage);

                response.Success.Should().BeFalse();

                Action<string> left = Fail.On<string>();
                Action<ErrorMessage> right = arg => arg.Should().Be(errorMessage);

                response.Data.Match(left, right);
            }
        }
    }
}

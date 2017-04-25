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
                response.Data.Left.Should().Be(responseText);
            }

            [Fact]
            public void CreateUnsuccessfulResponse()
            {
                var errorMessage = new ErrorMessage();
                var response = new ApiResponse<string>(errorMessage);

                response.Success.Should().BeFalse();
                response.Data.Right.Should().Be(errorMessage);
            }
        }
    }
}

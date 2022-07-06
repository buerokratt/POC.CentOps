using CentOps.Api.Authentication;
using FluentAssertions;

namespace CentOps.UnitTests.Authentication
{
    public class ApiKeyAuthenticationOptionsTests
    {
        [Fact]
        public void ApiKeyHeaderNameShouldBeDefaultValue()
        {
            var options = new ApiKeyAuthenticationOptions();

            _ = options.ApiKeyHeaderName.Should().Be("X-Api-Key");
        }

        [Fact]
        public void ApiKeyHeaderNameShouldBeSetValue()
        {
            var options = new ApiKeyAuthenticationOptions
            {
                ApiKeyHeaderName = "X-Custom-Key"
            };

            _ = options.ApiKeyHeaderName.Should().Be("X-Custom-Key");
        }
    }
}

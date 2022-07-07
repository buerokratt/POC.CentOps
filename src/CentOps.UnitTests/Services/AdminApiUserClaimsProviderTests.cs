using CentOps.Api.Configuration;
using CentOps.Api.Services;
using FluentAssertions;

namespace CentOps.UnitTests.Services
{
    public class AdminApiUserClaimsProviderTests
    {
        private readonly AdminApiUserClaimsProvider provider;
        private readonly AuthConfig config;

        public AdminApiUserClaimsProviderTests()
        {
            config = new AuthConfig();

            provider = new AdminApiUserClaimsProvider(config);
        }

        [Fact]
        public async Task ShouldReturnNullIfApiKeyNotFound()
        {
            // Arrange
            config.AdminApiKey = "dont-find-this";

            // Act
            var apiUser = await provider.GetUserClaimsAsync("non-existing-key").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().BeNull();
        }

        [Fact]
        public async Task ShouldReturnApiUserOfGivenKey()
        {
            // Arrange
            config.AdminApiKey = "find-this";

            // Act
            var apiUser = await provider.GetUserClaimsAsync("find-this").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().NotBeNull();

            var claims = apiUser.Claims;
            _ = claims.Should().HaveCount(1);

            var adminClaim = claims.ElementAt(0);
            _ = adminClaim.Type.Should().Be("admin");
            _ = adminClaim.Value.Should().Be(bool.TrueString);
        }
    }
}

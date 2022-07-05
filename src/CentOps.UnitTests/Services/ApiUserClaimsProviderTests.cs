using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using CentOps.Api.Services.ModelStore.Models;
using FluentAssertions;
using Moq;

namespace CentOps.UnitTests.Services
{
    public class ApiUserClaimsProviderTests
    {
        private readonly ApiUserClaimsProvider provider;
        private readonly Mock<IModelStore<IModel>> mockModelStore;

        public ApiUserClaimsProviderTests()
        {
            mockModelStore = new Mock<IModelStore<IModel>>();

            provider = new ApiUserClaimsProvider(mockModelStore.Object);
        }

        [Fact]
        public async Task ShouldReturnNullIfApiKeyNotFound()
        {
            // Arrange
            var user1 = SetupApiUser(apiKey: "qwerty");
            var user2 = SetupApiUser(name: "social", apiKey: "asdfg");
            SetModelStore(user1, user2);

            // Act
            var apiUser = await provider.GetUserClaimsAsync("non-existing-key").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().BeNull();
        }

        [Fact]
        public async Task ShouldReturnApiUserOfGivenKey()
        {
            // Arrange
            var user1 = SetupApiUser(apiKey: "qwerty", isAdmin: true);
            var user2 = SetupApiUser(name: "social", apiKey: "asdfg");
            SetModelStore(user1, user2);

            // Act
            var apiUser = await provider.GetUserClaimsAsync("qwerty").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().NotBeNull();

            var claims = apiUser.Claims;
            _ = claims.Should().HaveCount(2);

            var idClaim = claims.ElementAt(0);
            _ = idClaim.Type.Should().Be("id");
            _ = idClaim.Value.Should().Be(user1.Id);

            var adminClaim = claims.ElementAt(1);
            _ = adminClaim.Type.Should().Be("isAdmin");
            _ = adminClaim.Value.Should().Be("TRUE");
        }


        [Theory]
        [InlineData(true, "TRUE")]
        [InlineData(false, "FALSE")]
        public async Task ShouldReturnAdminClaimOfApiUser(bool isAdmin, string expectedIsAdminClaimValue)
        {
            // Arrange
            var user1 = SetupApiUser(apiKey: "qwerty", isAdmin: isAdmin);
            var user2 = SetupApiUser(name: "social", apiKey: "asdfg");
            SetModelStore(user1, user2);

            // Act
            var apiUser = await provider.GetUserClaimsAsync("qwerty").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().NotBeNull();

            var claims = apiUser.Claims;
            _ = claims.Should().HaveCount(2);

            var idClaim = claims.ElementAt(0);
            _ = idClaim.Type.Should().Be("id");
            _ = idClaim.Value.Should().Be(user1.Id);

            var adminClaim = claims.ElementAt(1);
            _ = adminClaim.Type.Should().Be("isAdmin");
            _ = adminClaim.Value.Should().Be(expectedIsAdminClaimValue);
        }

        private void SetModelStore(params IModel[] users)
        {
            _ = mockModelStore
                .Setup(m => m.GetByApiKeyAsync(It.IsAny<string>()))
                .Returns<string>(apiKey =>
                {
                    var apiUser = users.FirstOrDefault(x => x.ApiKey == apiKey);

                    return Task.FromResult(apiUser);
                });
        }

        private static IModel SetupApiUser(string id = null, string name = "nlib", string apiKey = "xyz", bool isAdmin = false)
        {
            return new InstitutionDto
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = name,
                ApiKey = apiKey,
                IsAdmin = isAdmin
            };
        }
    }
}

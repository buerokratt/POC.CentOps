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
        private readonly Mock<IParticipantStore> mockParticipantStore;

        public ApiUserClaimsProviderTests()
        {
            mockParticipantStore = new Mock<IParticipantStore>();

            provider = new ApiUserClaimsProvider(mockParticipantStore.Object);
        }

        [Fact]
        public async Task ShouldReturnNullIfApiKeyNotFound()
        {
            // Arrange
            var user1 = SetupApiUser(apiKey: "qwerty");
            var user2 = SetupApiUser(name: "social", apiKey: "asdfg");
            SetupParticipantStore(user1, user2);

            // Act
            var apiUser = await provider.GetUserClaimsAsync("non-existing-key").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().BeNull();
        }

        [Fact]
        public async Task ShouldReturnApiUserOfGivenKey()
        {
            // Arrange
            var user1 = SetupApiUser(id: "123", name: "health", institutionId: "inst:765", apiKey: "qwerty");
            var user2 = SetupApiUser(id: "234", name: "social", apiKey: "qazwsx");
            SetupParticipantStore(user1, user2);

            // Act
            var apiUser = await provider.GetUserClaimsAsync("qwerty").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().NotBeNull();

            var claims = apiUser.Claims;
            _ = claims.Should().HaveCount(4);

            var idClaim = claims.ElementAt(0);
            _ = idClaim.Type.Should().Be("id");
            _ = idClaim.Value.Should().Be("123");

            var nameClaim = claims.ElementAt(1);
            _ = nameClaim.Type.Should().Be("name");
            _ = nameClaim.Value.Should().Be("health");

            var institutionClaim = claims.ElementAt(2);
            _ = institutionClaim.Type.Should().Be("institutionId");
            _ = institutionClaim.Value.Should().Be("inst:765");

            var statusClaim = claims.ElementAt(3);
            _ = statusClaim.Type.Should().Be("status");
            _ = statusClaim.Value.Should().Be(ParticipantStatusDto.Active.ToString());
        }

        [Fact]
        public async Task ShouldReturnNullIfParticipantIsDisabled()
        {
            // Arrange
            var user1 = SetupApiUser(id: "123", name: "health", status: ParticipantStatusDto.Disabled, apiKey: "disabled");
            var user2 = SetupApiUser(id: "234", name: "social", apiKey: "qazwsx");
            SetupParticipantStore(user1, user2);

            // Act
            var apiUser = await provider.GetUserClaimsAsync("disabled").ConfigureAwait(false);

            // Assert
            _ = apiUser.Should().BeNull();
        }

        private void SetupParticipantStore(params ParticipantDto[] participants)
        {
            _ = mockParticipantStore
                .Setup(m => m.GetByApiKeyAsync(It.IsAny<string>()))
                .Returns<string>(apiKey =>
                {
                    var apiUser = participants.FirstOrDefault(x => x.ApiKey == apiKey);

                    return Task.FromResult(apiUser);
                });
        }

        private static ParticipantDto SetupApiUser(
            string id = null,
            string name = "nlib",
            string institutionId = null,
            string apiKey = "xyz",
            ParticipantStatusDto status = ParticipantStatusDto.Active)
        {
            return new ParticipantDto
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Name = name,
                InstitutionId = institutionId ?? Guid.NewGuid().ToString(),
                ApiKey = apiKey,
                Status = status
            };
        }
    }
}

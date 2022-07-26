using CentOps.Api.Extensions;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace CentOps.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection services = new ServiceCollection();

        [Fact]
        public void ShouldAddDataStoreDependencies()
        {
            services.AddInMemoryDataStores();

            var interfaces = services.Select(d => d.ServiceType);

            _ = interfaces.Should().Contain(typeof(IInstitutionStore));
            _ = interfaces.Should().Contain(typeof(IParticipantStore));
        }

        [Fact]
        public async Task ShouldSetParticipantAndAdminAuthorizationPolicies()
        {
            services.AddAuthorizationPolicies();
            var provider = services.BuildServiceProvider();

            var policyProvider = provider.GetService<IAuthorizationPolicyProvider>();

            // Assert participany policy
            var participantPolicy = await policyProvider.GetPolicyAsync("ParticipantPolicy").ConfigureAwait(false);

            _ = participantPolicy.AuthenticationSchemes.Should()
                .HaveCount(1)
                .And
                .Contain("ApiKey");

            var requirements = participantPolicy.Requirements;
            _ = requirements.Should().HaveCount(1);

            var idClaim = requirements[0] as ClaimsAuthorizationRequirement;
            _ = idClaim.Should().NotBeNull();
            _ = idClaim.ClaimType.Should().Be("id");
            _ = idClaim.AllowedValues.Should().BeNullOrEmpty();

            // Assert admin policy
            var adminPolicy = await policyProvider.GetPolicyAsync("AdminPolicy").ConfigureAwait(false);

            _ = adminPolicy.AuthenticationSchemes.Should()
                .HaveCount(1)
                .And
                .Contain("AdminApiKey");

            requirements = adminPolicy.Requirements;
            _ = requirements.Should().HaveCount(1);

            var adminClaim = requirements[0] as ClaimsAuthorizationRequirement;
            _ = adminClaim.Should().NotBeNull();
            _ = adminClaim.ClaimType.Should().Be("admin");
            _ = adminClaim.AllowedValues.Should().BeEquivalentTo(bool.TrueString);
        }

        [Fact]
        public void AddCosmosDbServicesShouldThrowWhenConfigSectionIsNull()
        {
            Action action = () => services.AddCosmosDbServices(null);
            _ = action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void AddCosmosDbServicesShouldAddCosmosServicesToServiceCollection()
        {
            // Arrange
            var configSection = new Mock<IConfigurationSection>();
            _ = configSection.Setup(m => m["DatabaseName"]).Returns("fakedatabasename");
            _ = configSection.Setup(m => m["ContainerName"]).Returns("fakecontainername");
            _ = configSection.Setup(m => m["Account"]).Returns("https://fakeaccount.documents.azure.com:443/");
            _ = configSection.Setup(m => m["Key"]).Returns("ZmFrZWtleQ=="); // Base64 of "fakekey"

            // Act
            services.AddCosmosDbServices(configSection.Object);
            var provider = services.BuildServiceProvider();

            // Assert

            // Ensure that the services get registered
            var interfaces = services.Select(s => s.ServiceType).ToArray();
            _ = interfaces.Should().Contain(typeof(IInstitutionStore));
            _ = interfaces.Should().Contain(typeof(IParticipantStore));
            _ = interfaces.Should().Contain(typeof(CosmosDbService));

            // Ensure that the same CosmosDbService object is used for both stores1
            var institutionStore = provider.GetRequiredService<IInstitutionStore>();
            var participantStore = provider.GetRequiredService<IParticipantStore>();
            var cosmosDbService = provider.GetRequiredService<CosmosDbService>();

            _ = cosmosDbService.Should().BeSameAs(institutionStore);
            _ = cosmosDbService.Should().BeSameAs(participantStore);
        }

        [Fact]
        public void AddCorsShouldThrowWhenConfigIsNull()
        {
            // Act & Assert
            Action action = () => services.AddCorsConfiguration(null);
            _ = action.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void AddCorsShouldAddCorrectConfiguration()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"Cors:AllowedOrigins", "*"},
            };

            IConfiguration configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(inMemorySettings)
               .Build();

            IServiceCollection services = new ServiceCollection();

            // Act
            services.AddCorsConfiguration(configuration);

            // Assert
            var interfaces = services.Select(s => s.ServiceType).ToArray();
            _ = interfaces.Should().Contain(typeof(ICorsService));
            _ = interfaces.Should().Contain(typeof(ICorsPolicyProvider));
        }
    }
}

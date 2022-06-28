using CentOps.Api.Extensions;
using CentOps.Api.Services.ModelStore.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CentOps.UnitTests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection services = new ServiceCollection();

        [Fact]
        public void ShouldAddDataStoreDependencies()
        {
            services.AddDataStores();

            var interfaces = services.Select(d => d.ServiceType);

            _ = interfaces.Should().Contain(typeof(IApiUserStore));
            _ = interfaces.Should().Contain(typeof(IInstitutionStore));
            _ = interfaces.Should().Contain(typeof(IParticipantStore));
        }

        [Fact]
        public async Task ShouldSetUserAndDefaultAuthorizationPolicy()
        {
            services.AddAuthorizationPolicies();
            var provider = services.BuildServiceProvider();

            var policyProvider = provider.GetService<IAuthorizationPolicyProvider>();

            var userPolicy = await policyProvider.GetPolicyAsync("UserPolicy").ConfigureAwait(false);
            var defaultPolicy = await policyProvider.GetDefaultPolicyAsync().ConfigureAwait(false);

            _ = userPolicy.AuthenticationSchemes.Should()
                .HaveCount(1)
                .And
                .Contain("ApiKey");

            var requirements = userPolicy.Requirements;
            _ = requirements.Should().HaveCount(1);

            var idClaim = requirements[0] as ClaimsAuthorizationRequirement;
            _ = idClaim.Should().NotBeNull();
            _ = idClaim.ClaimType.Should().Be("id");
            _ = idClaim.AllowedValues.Should().BeNullOrEmpty();

            _ = defaultPolicy.Should().BeSameAs(userPolicy);
        }

        [Fact]
        public async Task ShouldSetAdminAuthorizationPolicy()
        {
            services.AddAuthorizationPolicies();
            var provider = services.BuildServiceProvider();

            var policyProvider = provider.GetService<IAuthorizationPolicyProvider>();

            var adminPolicy = await policyProvider.GetPolicyAsync("AdminPolicy").ConfigureAwait(false);

            _ = adminPolicy.AuthenticationSchemes.Should()
                .HaveCount(1)
                .And
                .Contain("ApiKey");

            var requirements = adminPolicy.Requirements;
            _ = requirements.Should().HaveCount(2);

            var idClaim = requirements[0] as ClaimsAuthorizationRequirement;
            _ = idClaim.Should().NotBeNull();
            _ = idClaim.ClaimType.Should().Be("id");
            _ = idClaim.AllowedValues.Should().BeNullOrEmpty();

            var adminClaim = requirements[1] as ClaimsAuthorizationRequirement;
            _ = adminClaim.Should().NotBeNull();
            _ = adminClaim.ClaimType.Should().Be("isAdmin");
            _ = adminClaim.AllowedValues.Should().BeEquivalentTo("TRUE");

        }
    }
}

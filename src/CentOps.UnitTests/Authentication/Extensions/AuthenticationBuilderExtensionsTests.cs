using CentOps.Api.Authentication;
using CentOps.Api.Authentication.Extensions;
using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace CentOps.UnitTests.Authentication.Extensions
{
    public class AuthenticationBuilderExtensionsTests
    {
        [Fact]
        public void ServiceCollectionShouldContainApiUserClaimsProvider()
        {
            // Arrange
            var builder = GetAuthenticationBuilder();

            // Act
            builder.AddApiKeyAuth<ApiUserClaimsProvider>();

            // Assert
            var provider = builder.Services.BuildServiceProvider();
            var claimsProvider = provider.GetService<IApiUserClaimsProvider>();

            _ = claimsProvider.Should().NotBeNull();
        }

        private static AuthenticationBuilder GetAuthenticationBuilder()
        {
            var services = new ServiceCollection();
            _ = services.AddSingleton(new Mock<IApiUserStore>().Object);

            var logger = new Mock<ILogger<ApiKeyAuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>();
            _ = loggerFactory
                .Setup(m => m.CreateLogger(It.IsAny<string>()))
                .Returns(logger.Object);

            _ = services.AddSingleton(loggerFactory.Object);

            var builder = services.AddAuthentication();

            return builder;
        }
    }
}

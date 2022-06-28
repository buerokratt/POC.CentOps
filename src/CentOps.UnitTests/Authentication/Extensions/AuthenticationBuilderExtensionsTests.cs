using CentOps.Api.Authentication;
using CentOps.Api.Authentication.Extensions;
using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
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

        //[Theory]
        //[InlineData("", "X-Api-Key")]
        //[InlineData(null, "X-Api-Key")]
        //[InlineData("X-Custom-Key", "X-Custom-Key")]
        //public async Task SchemeShouldUseGivenApiKeyHeaderName(string apiKeyHeaderName, string expectedHeaderName)
        //{
        //    // Arrange
        //    var builder = GetAuthenticationBuilder();

        //    // Act
        //    builder.AddApiKeyAuth<ApiUserClaimsProvider>(apiKeyHeaderName);

        //    // Assert
        //    var provider = builder.Services.BuildServiceProvider();
        //    var schemeProvider = provider.GetService<IAuthenticationSchemeProvider>();
        //    //var handler = provider.GetRequiredService<ApiKeyAuthenticationHandler>();
        //    var handlerProvider = provider.GetRequiredService<IAuthenticationHandlerProvider>();
        //    var handler = await GetAuthenticationHandler(handlerProvider).ConfigureAwait(false);

        //    await handler.Ini

        //    var scheme = await schemeProvider.GetSchemeAsync("ApiKey").ConfigureAwait(false);
        //    _ = scheme.Name.Should().Be("ApiKey");
        //    _ = scheme.HandlerType.Should().BeOfType(typeof(ApiKeyAuthenticationHandler).GetType());

        //    _ = expectedHeaderName;
        //    _ = handlerProvider;
        //}


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

        //private static Task<IAuthenticationHandler> GetAuthenticationHandler(IAuthenticationHandlerProvider provider)
        //{
        //    return provider.GetHandlerAsync(new DefaultHttpContext(), ApiKeyAuthenciationDefaults.AuthenticationScheme);
        //}

        //private static 
    }
}

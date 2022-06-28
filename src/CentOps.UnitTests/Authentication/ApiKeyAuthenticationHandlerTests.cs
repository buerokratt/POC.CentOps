using CentOps.Api.Authentication;
using CentOps.Api.Authentication.Interfaces;
using CentOps.Api.Authentication.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Moq;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace CentOps.UnitTests.Authentication
{
    public class ApiKeyAuthenticationHandlerTests
    {
        private static readonly AuthenticationScheme AuthScheme = new(
            "ApiKey",
            null,
            typeof(ApiKeyAuthenticationHandler).BaseType);

        private readonly ApiKeyAuthenticationHandler handler;

        private readonly Mock<IApiUserClaimsProvider> mockProvider;
        private readonly Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>> mockOptions;
        private readonly Mock<ILoggerFactory> mockLoggerFactory;
        private readonly Mock<ISystemClock> mockSystemClock;

        public ApiKeyAuthenticationHandlerTests()
        {
            mockProvider = new Mock<IApiUserClaimsProvider>();
            mockOptions = new Mock<IOptionsMonitor<ApiKeyAuthenticationOptions>>();
            mockLoggerFactory = new Mock<ILoggerFactory>();
            mockSystemClock = new Mock<ISystemClock>();

            SetupOptions();
            SetupLogger();

            handler = new ApiKeyAuthenticationHandler(
                mockProvider.Object,
                mockOptions.Object,
                mockLoggerFactory.Object,
                UrlEncoder.Default,
                mockSystemClock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task ReturnsFailedWhenHeaderValueIsNullOrEmpty(string apiKeyValue)
        {
            // Arrange
            var httpContext = GetHttpContext(apiKeyValue);
            await handler.InitializeAsync(AuthScheme, httpContext).ConfigureAwait(false);

            // Act
            var result = await handler.AuthenticateAsync().ConfigureAwait(false);

            // Assert
            _ = result.Succeeded.Should().BeFalse();
            _ = result.Principal.Should().BeNull();
            _ = result.Ticket.Should().BeNull();

            var failure = result.Failure;
            _ = failure.Should().NotBeNull();
            _ = failure.Message.Should().Be("The 'X-Api-Key' header does not contain a value.");
        }

        [Fact]
        public async Task ReturnsFailedWhenApiKeyNotFound()
        {
            // Arrange
            var httpContext = GetHttpContext("non-existing-key");

            SetupClaimsProvider(new Dictionary<string, ApiUser>
            {
                { "apikey1", new ApiUser(new[] { new Claim("type1", "value1") }) }
            });

            await handler.InitializeAsync(AuthScheme, httpContext).ConfigureAwait(false);

            // Act
            var result = await handler.AuthenticateAsync().ConfigureAwait(false);

            // Assert
            _ = result.Succeeded.Should().BeFalse();
            _ = result.Principal.Should().BeNull();
            _ = result.Ticket.Should().BeNull();

            var failure = result.Failure;
            _ = failure.Should().NotBeNull();
            _ = failure.Message.Should().Be("The 'X-Api-Key' header contains an invalid API key.");
        }

        [Fact]
        public async Task ReturnsSuccessWhenApiKeyIsFound()
        {
            // Arrange
            var httpContext = GetHttpContext("validkey1");

            SetupClaimsProvider(new Dictionary<string, ApiUser>
            {
                { "validkey1", new ApiUser(new[] { new Claim("type1", "value1") }) },
                { "validkey2", new ApiUser(new[] { new Claim("type2", "value2") }) }
            });

            await handler.InitializeAsync(AuthScheme, httpContext).ConfigureAwait(false);

            // Act
            var result = await handler.AuthenticateAsync().ConfigureAwait(false);

            // Assert
            _ = result.Succeeded.Should().BeTrue();
            _ = result.Principal.Should().NotBeNull();
            _ = result.Ticket.Should().NotBeNull();
            _ = result.Failure.Should().BeNull();

            var claims = result.Principal.Claims;
            _ = claims.Should().HaveCount(1);

            var claim = claims.ElementAt(0);
            _ = claim.Type.Should().Be("type1");
            _ = claim.Value.Should().Be("value1");
        }

        private void SetupOptions(string apiKeyHeaderName = "X-Api-Key")
        {
            _ = mockOptions
                .Setup(m => m.Get(It.IsAny<string>()))
                .Returns(new ApiKeyAuthenticationOptions
                {
                    ApiKeyHeaderName = apiKeyHeaderName
                });
        }

        private void SetupLogger()
        {
            var logger = new Mock<ILogger<ApiKeyAuthenticationHandler>>();

            _ = mockLoggerFactory
                .Setup(m => m.CreateLogger(It.IsAny<string>()))
                .Returns(logger.Object);
        }

        private void SetupClaimsProvider(IDictionary<string, ApiUser> users)
        {
            if (mockProvider == null)
            {
                return;
            }

#nullable enable
            _ = mockProvider
                .Setup(m => m.GetUserClaimsAsync(It.IsAny<string>()))
                .Returns<string>(apiKey =>
                {
                    return users.ContainsKey(apiKey)
                        ? Task.FromResult<ApiUser?>(users[apiKey])
                        : Task.FromResult<ApiUser?>(null);
                });
#nullable disable
        }

        private HttpContext GetHttpContext(string apiKeyValue)
        {
            var context = new DefaultHttpContext();

            if (apiKeyValue != null)
            {
                var headerKey = mockOptions.Object.Get("Test").ApiKeyHeaderName;
                var headerValue = new StringValues(apiKeyValue);
                context.Request.Headers.Add(headerKey, headerValue);
            }

            return context;
        }
    }
}

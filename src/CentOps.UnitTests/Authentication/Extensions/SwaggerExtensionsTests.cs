using CentOps.Api.Authentication.Extensions;
using FluentAssertions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CentOps.UnitTests.Authentication.Extensions
{
    public class SwaggerExtensionsTests
    {
        [Fact]
        public void ShouldThrowIfOptionsAreNull()
        {
            // Arrange
            SwaggerGenOptions options = null;

            // Act
            Action action = () => options.AddApiKeyOpenApiSecurity();

            // Assert
            _ = action.Should()
                .ThrowExactly<ArgumentNullException>()
                .WithParameterName("options");
        }

        [Theory]
        [InlineData("", "X-Api-Key")]
        [InlineData(null, "X-Api-Key")]
        [InlineData("X-Custom-Key", "X-Custom-Key")]
        public void ShouldAddApiKeySecurityWithGivenApiKeyHeaderNames(string headerName, string expectedHeaderName)
        {
            // Arrange and Act
            var extendedOptions = new SwaggerGenOptions().AddApiKeyOpenApiSecurity(headerName);

            // Assert
            var options = extendedOptions.SwaggerGeneratorOptions;

            var schemes = options.SecuritySchemes;
            _ = schemes.Should().ContainKey("ApiKey");

            var scheme = schemes["ApiKey"];
            _ = scheme.Scheme.Should().Be("ApiKey");
            _ = scheme.Name.Should().Be(expectedHeaderName);
            _ = scheme.Type.Should().Be(SecuritySchemeType.ApiKey);
            _ = scheme.In.Should().Be(ParameterLocation.Header);
            _ = scheme.Description.Should().Be($"API Key authentication using key set in the value of the '{expectedHeaderName}' header.");

            var reference = scheme.Reference;

            _ = reference.Should().NotBeNull();
            _ = reference.Id.Should().Be("ApiKey");
            _ = reference.Type.Should().Be(ReferenceType.SecurityScheme);

            var requirements = options.SecurityRequirements;
            _ = requirements.Should().HaveCount(1);

            var requirement = requirements.First();
            _ = requirement.Should().ContainKey(scheme);
        }
    }
}

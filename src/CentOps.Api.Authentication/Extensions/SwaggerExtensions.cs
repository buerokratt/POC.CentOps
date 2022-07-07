using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CentOps.Api.Authentication.Extensions
{
    public static class SwaggerExtensions
    {
        public static SwaggerGenOptions AddApiKeyOpenApiSecurity(this SwaggerGenOptions options, string? apiKeyHeaderName = null)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (string.IsNullOrEmpty(apiKeyHeaderName))
            {
                apiKeyHeaderName = ApiKeyAuthenticationDefaults.DefaultApiKeyHeaderName;
            }

            var userSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = ApiKeyAuthenticationDefaults.AuthenticationScheme,
                Name = apiKeyHeaderName,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = $"API Key authentication using key set in the value of the '{apiKeyHeaderName}' header.",
                Reference = new OpenApiReference
                {
                    Id = ApiKeyAuthenticationDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(userSecurityScheme.Scheme, userSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { userSecurityScheme, Array.Empty<string>() }
            });

            return options;
        }
    }
}

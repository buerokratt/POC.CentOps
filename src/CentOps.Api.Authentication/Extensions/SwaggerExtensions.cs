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
                apiKeyHeaderName = ApiKeyAuthenciationDefaults.DefaultApiKeyHeaderName;
            }

            var securityScheme = new OpenApiSecurityScheme
            {
                Scheme = ApiKeyAuthenciationDefaults.AuthenticationScheme,
                Name = apiKeyHeaderName,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = $"API Key authentication using key set in the value of the '{apiKeyHeaderName}' header.",
                Reference = new OpenApiReference
                {
                    Id = ApiKeyAuthenciationDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityDefinition(securityScheme.Scheme, securityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });

            return options;
        }
    }
}

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

            var adminSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = ApiKeyAuthenticationDefaults.AdminAuthenticationScheme,
                Name = apiKeyHeaderName,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = $"API Key authentication for Admin only using key set in the value of the '{apiKeyHeaderName}' header.",
                Reference = new OpenApiReference
                {
                    Id = ApiKeyAuthenticationDefaults.AdminAuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

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

            options.AddSecurityDefinition(adminSecurityScheme.Scheme, adminSecurityScheme);
            options.AddSecurityDefinition(userSecurityScheme.Scheme, userSecurityScheme);

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { adminSecurityScheme, Array.Empty<string>() },
                { userSecurityScheme, Array.Empty<string>() }
            });

            return options;
        }
    }
}

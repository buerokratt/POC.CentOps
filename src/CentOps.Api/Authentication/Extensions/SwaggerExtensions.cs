using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CentOps.Api.Authentication.Extensions
{
    public static class SwaggerExtensions
    {
        public static SwaggerGenOptions AddApiKeyOpenApiSecurity(this SwaggerGenOptions options)
        {
            options.AddSecurityDefinition(ApiKeyAuthenciationDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Scheme = ApiKeyAuthenciationDefaults.AuthenticationScheme,
                Name = ApiKeyAuthenciationDefaults.DefaultApiKeyHeaderName,
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Description = $"API Key authentication using key set in the value of the ${ApiKeyAuthenciationDefaults.DefaultApiKeyHeaderName} header."
            });

            var securityScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = ApiKeyAuthenciationDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, Array.Empty<string>() }
            });

            return options;
        }
    }
}

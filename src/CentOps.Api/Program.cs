using CentOps.Api.Authentication;
using CentOps.Api.Authentication.Extensions;
using CentOps.Api.Services;
using System.Diagnostics.CodeAnalysis;

namespace CentOps.Api
{
    [ExcludeFromCodeCoverage] // This is not solution code, no need for unit tests
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var services = builder.Services;

            _ = services.AddSingleton<IAuthService, AuthService>();

            _ = services
                .AddSingleton<IParticipantStore, ParticipantStore>()
                .AddSingleton<IInsitutionStore, InstitutionStore>();

            _ = services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = services.AddEndpointsApiExplorer();
            _ = services.AddSwaggerGen(options =>
            {
                _ = options.AddApiKeyOpenApiSecurity();
            });

            services.AddApiKeyAuthentication();

            _ = services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminPolicy", builder =>
                {
                    _ = builder
                        .AddAuthenticationSchemes(ApiKeyAuthenciationDefaults.AuthenticationScheme)
                        .RequireClaim("isAdmin", "TRUE");
                });

                options.AddPolicy("UserPolicy", builder =>
                {
                    _ = builder
                        .AddAuthenticationSchemes(ApiKeyAuthenciationDefaults.AuthenticationScheme)
                        .RequireClaim("id");
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI();
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseAuthentication();
            _ = app.UseAuthorization();

            _ = app.MapControllers();

            app.Run();
        }
    }
}
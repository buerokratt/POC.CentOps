
using CentOps.Api.Authentication;
using CentOps.Api.Authentication.Extensions;
using CentOps.Api.Services;
using CentOps.Api.Services.ModelStore.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

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

            _ = services.AddControllers().AddJsonOptions(jo =>
            {
                jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            _ = builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

            var inMemoryStore = new InMemoryStore();
            _ = services.AddSingleton<IInstitutionStore>(provider => inMemoryStore);
            _ = services.AddSingleton<IParticipantStore>(provider => inMemoryStore);

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
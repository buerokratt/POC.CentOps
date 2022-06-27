
using CentOps.Api.Authentication.Extensions;
using CentOps.Api.Extensions;
using CentOps.Api.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
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

            _ = services
                .AddControllers(options => options.Filters.Add(new AuthorizeFilter()))
                .AddJsonOptions(jo =>
                {
                    jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            _ = services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            _ = services.AddEndpointsApiExplorer();
            _ = services.AddSwaggerGen(options =>
            {
                _ = options.AddApiKeyOpenApiSecurity();
            });

            services.AddApiKeyAuthentication<ApiUserClaimsProvider>();
            services.AddAuthorizationPolicies();

            services.AddDataStores();

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
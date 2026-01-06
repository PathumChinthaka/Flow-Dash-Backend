using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Infrastructure.Services;
using FlowDash.Infrastructure.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Npgsql;
using System.Data;

namespace FlowDash.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            // Settings
            services.Configure<ServerSettings>(configuration.GetSection(ServerSettings.SectionName));

            var corsSettings = new CorsSettings();
            configuration.Bind(CorsSettings.SectionName, corsSettings);

            corsSettings.AllowedOrigins = JsonConvert.DeserializeObject<string[]>(configuration["CorsSettings:AllowedOrigins"] ?? "") ?? Array.Empty<string>();
            corsSettings.AllowedEndPatterns = JsonConvert.DeserializeObject<string[]>(configuration["CorsSettings:AllowedEndPatterns"] ?? "") ?? Array.Empty<string>();

            services.AddSingleton(Options.Create(corsSettings));

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: corsSettings.PolicyName,
                    policy =>
                    {
                        policy
                        .SetIsOriginAllowed(origin => IsMatched(new Uri(origin).Host, corsSettings.AllowedOrigins, corsSettings.AllowedEndPatterns))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Services
            services.AddScoped<ITenantIdentifierService, TenantIdentifierService>();
            services.AddScoped<ITenantCreationService, TenantCreationService>();

            // DB Connection
            services.AddDbContext<FlowDashDbContext>(option => option.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")
            ));

            // Dapper DB Connection
            services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        private static bool IsMatched(string origin, string[] allowedOrigins, string[] allowedEndPatterns)
        {
            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return false;

            var host = uri.Host;

            if (allowedOrigins.Contains(host, StringComparer.OrdinalIgnoreCase))
                return true;

            return allowedEndPatterns.Any(pattern =>
                host.Equals(pattern, StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith("." + pattern, StringComparison.OrdinalIgnoreCase));
        }
    }
}

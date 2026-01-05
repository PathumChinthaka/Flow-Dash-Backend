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
            try
            {
                var uri = new Uri($"https://{origin.Trim()}"); // Ensure it has a valid scheme for parsing
                string host = uri.Host;

                // Exact match with allowedOrigins
                if (allowedOrigins.Contains(host))
                {
                    return true;
                }

                // Allow all subdomains of allowedEndPatterns (e.g., *.example.com)
                foreach (var pattern in allowedEndPatterns)
                {
                    if (host == pattern) // If the exact domain matches
                    {
                        return true;
                    }

                    if (host.EndsWith("." + pattern)) // Only allow valid subdomains, not just string matches
                    {
                        return true;
                    }
                }
            }
            catch
            {
                return false; // Invalid origin format (e.g., missing scheme)
            }

            return false;
        }
    }
}

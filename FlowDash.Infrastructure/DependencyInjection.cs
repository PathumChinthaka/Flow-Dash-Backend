using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Infrastructure.Services;
using FlowDash.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using System.Text;

namespace FlowDash.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddAuth(configuration);
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

        public static IServiceCollection AddAuth(this IServiceCollection services, ConfigurationManager configuration)
        {
            // JWT settings
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));

            //services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            var jwtSettings = configuration
                .GetSection(JwtSettings.SectionName)
                .Get<JwtSettings>() ?? throw new InvalidOperationException("JwtSettings not configured");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "SmartScheme";
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                // 🔀 Scheme selector
                .AddPolicyScheme("SmartScheme", "Smart Auth Scheme", options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        var authorization = context.Request.Headers.Authorization.ToString();

                        if (authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            return JwtBearerDefaults.AuthenticationScheme;

                        return JwtBearerDefaults.AuthenticationScheme;
                    };
                })

                // 🔐 JWT
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
                    };
                });

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

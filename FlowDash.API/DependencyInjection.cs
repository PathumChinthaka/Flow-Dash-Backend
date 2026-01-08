using FlowDash.API.Common.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace FlowDash.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services, ConfigurationManager configuration)
        {
            var apiVersion = configuration.GetValue<string>("ApiVersion");

            services.AddControllers(options =>
            {
                options.Conventions.Insert(0, new RoutePrefixConvention(new RouteAttribute($"{apiVersion}")));
            });

            services.AddEndpointsApiExplorer();
            
            services.AddMappings();
            services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.EnableAnnotations();
                c.SwaggerDoc(apiVersion, new OpenApiInfo
                {
                    Title = "FlowDash.API",
                    Version = apiVersion,
                    Description = "These are the main API endpoints of the App",
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter 'Bearer' followed by a space and the JWT token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new List<string>()
                    }
                });
            });

            return services;
        }
    }
}

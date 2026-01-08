using FlowDash.API;
using FlowDash.API.Middleware;
using FlowDash.Application;
using FlowDash.Infrastructure;
using FlowDash.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

// Add services to the container.
builder.Services
        .AddInfrastructure(builder.Configuration)
        .AddPresentation(builder.Configuration)
        .AddApplication(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<FlowDashDbContext>();
    await dbContext.Database.MigrateAsync();
}

var corsSettings = app.Services.GetRequiredService<IOptions<CorsSettings>>().Value;

if (app.Environment.IsDevelopment())
{
    var apiVersion = app.Configuration.GetValue<string>("ApiVersion");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"API {apiVersion}");
    });
}

app.UseRouting();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseCors(corsSettings.PolicyName);

app.UseAuthentication();

app.UseMiddleware<TenantValidationMiddleware>();

app.UseAuthorization();

app.UseHttpsRedirection();

app.MapControllers();

await app.RunAsync();

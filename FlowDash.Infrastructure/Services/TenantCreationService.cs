using Dapper;
using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Exceptions.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Text.RegularExpressions;

namespace FlowDash.Infrastructure.Services
{
    public sealed class TenantCreationService : ITenantCreationService
    {
        private readonly IDbConnection _db;
        private readonly ITenantIdentifierService _tenantIdentifier;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TenantCreationService> _logger;

        private static readonly Regex TenantNameRegex = new(@"^[a-z][a-z0-9_]{2,30}$", RegexOptions.IgnoreCase);

        public TenantCreationService
        (
            IDbConnection db,
            ITenantIdentifierService tenantIdentifier,
            IConfiguration configuration,
            ILogger<TenantCreationService> logger
        )
        {
            _db = db;
            _tenantIdentifier = tenantIdentifier;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task CreateSchema(string tenantName)
        {
            ValidateTenantName(tenantName);

            using var tx = _db.BeginTransaction();

            try
            {
                await AcquireAdvisoryLock(tx);

                if (await SchemaExists(tenantName, tx))
                    throw new InternalServerException("Tenant schema already exists.");

                await _db.ExecuteAsync(
                    $"CREATE SCHEMA \"{tenantName}\" AUTHORIZATION CURRENT_USER;",
                    transaction: tx);

                await ApplyMigrationsToSchema(tenantName);

                tx.Commit();

                _logger.LogInformation("Tenant schema {Tenant} created successfully", tenantName);
            }
            catch (Exception ex)
            {
                tx.Rollback();
                _logger.LogError(ex, "Failed to create tenant schema {Tenant}", tenantName);
                throw new InternalServerException("Failed to create tenant schema");
            }
        }

        public async Task DeleteSchema(string tenantName)
        {
            ValidateTenantName(tenantName);

            using var tx = _db.BeginTransaction();

            try
            {
                if (!await SchemaExists(tenantName, tx))
                    throw new InternalServerException("Tenant schema does not exist.");

                await _db.ExecuteAsync(
                    $"DROP SCHEMA \"{tenantName}\" CASCADE;",
                    transaction: tx);

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                _logger.LogError(ex, "Failed to delete tenant schema {Tenant}", tenantName);
                throw new InternalServerException("Failed to delete tenant schema");
            }
        }

        public async Task UpdateDatabase()
        {
            _tenantIdentifier.SetCurrentTenantName("public");

            using var tx = _db.BeginTransaction();

            try
            {
                await AcquireAdvisoryLock(tx);

                using var dbContext = new FlowDashDbContext(_tenantIdentifier, _configuration);

                var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
                if (!pendingMigrations.Any())
                    return;

                var schemas = await GetTenantSchemas(tx);

                foreach (var schema in schemas)
                {
                    await ApplyMigrationsToSchema(schema);
                }

                tx.Commit();
            }
            catch (Exception ex)
            {
                tx.Rollback();
                _logger.LogError(ex, "Database migration failed");
                throw new InternalServerException("Database migration failed");
            }
        }

        private async Task ApplyMigrationsToSchema(string schema)
        {
            _tenantIdentifier.SetCurrentTenantName(schema);

            using var dbContext = new FlowDashDbContext(_tenantIdentifier, _configuration);

            var migrator = dbContext.GetService<IMigrator>();

            _logger.LogInformation("Applying migrations to schema {Schema}", schema);

            // EF Core handles migration history internally per schema
            await migrator.MigrateAsync();
        }

        private async Task<bool> SchemaExists(string schema, IDbTransaction tx)
        {
            return await _db.QuerySingleAsync<bool>(
                "SELECT EXISTS (SELECT 1 FROM information_schema.schemata WHERE schema_name = @schema)",
                new { schema },
                transaction: tx);
        }

        private async Task<List<string>> GetTenantSchemas(IDbTransaction tx)
        {
            return (await _db.QueryAsync<string>(
                @"SELECT schema_name
                  FROM information_schema.schemata
                  WHERE schema_name NOT IN ('public','information_schema')
                    AND schema_name NOT LIKE 'pg_%'
                    AND schema_name <> 'hangfire';",
                transaction: tx))
                .ToList();
        }

        private static void ValidateTenantName(string tenantName)
        {
            if (!TenantNameRegex.IsMatch(tenantName))
                throw new InternalServerException("Invalid tenant name format.");
        }

        private async Task AcquireAdvisoryLock(IDbTransaction tx)
        {
            await _db.ExecuteAsync(
                "SELECT pg_advisory_lock(hashtext('tenant_migration_lock'));",
                transaction: tx);
        }
    }
}

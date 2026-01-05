using Dapper;
using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Application.Exceptions.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace FlowDash.Infrastructure.Services
{
    public class TenantCreationService : ITenantCreationService
    {
        private readonly IDbConnection _dbConnection;
        private readonly FlowDashDbContext _context;
        private readonly ITenantIdentifierService _tenantIdentifier;
        private readonly IConfiguration _configuration;

        public TenantCreationService
        (
            FlowDashDbContext context,
            ITenantIdentifierService tenantIdentifier,
            IConfiguration configuration,
            IDbConnection dbConnection
        )
        {
            _context = context;
            _dbConnection = dbConnection;
            _tenantIdentifier = tenantIdentifier;
            _configuration = configuration;
        }

        public async Task CreateSchema(string tenantName)
        {
            var schemaExists = await _dbConnection.QueryFirstOrDefaultAsync<bool>(
                $"SELECT EXISTS(SELECT 1 FROM information_schema.schemata WHERE schema_name = '{tenantName}')");

            if (!schemaExists)
            {
                try
                {
                    await _dbConnection.ExecuteAsync($"CREATE SCHEMA \"{tenantName}\"");

                    var script = _context.Database.GenerateCreateScript();
                    script = Regex.Replace(script, $"CREATE SCHEMA {_tenantIdentifier.GetCurrentTenantName()}", $"CREATE SCHEMA {tenantName}");
                    script = Regex.Replace(script, "CREATE TABLE", "CREATE TABLE IF NOT EXISTS");
                    script = script.Replace("CREATE EXTENSION IF NOT EXISTS pg_buffercache;", "");
                    script = script.Replace("CREATE EXTENSION IF NOT EXISTS pg_stat_statements;", "");
                    script = script.Replace("CREATE EXTENSION IF NOT EXISTS pg_prewarm;", "");
                    script = script.Replace("CREATE EXTENSION IF NOT EXISTS pg_stat_statements;", "");

                    await _context.Database.ExecuteSqlRawAsync(script);
                }
                catch (Exception ex)
                {
                    await DeleteSchema(tenantName);
                    throw new InternalServerException(ex.Message);
                }
            }
            else
            {
                throw new InternalServerException("Schema already exists");
            }
        }

        public async Task DeleteSchema(string tenantName)
        {
            try
            {
                var schemaExists = await _dbConnection.QueryFirstOrDefaultAsync<bool>(
                    $"SELECT EXISTS(SELECT 1 FROM information_schema.schemata WHERE schema_name = '{tenantName}')",
                    new { tenantName });

                if (schemaExists)
                {
                    await _dbConnection.ExecuteAsync($"DROP SCHEMA IF EXISTS \"{tenantName}\" CASCADE");
                }

                else
                {
                    throw new InternalServerException("Schema Not exists");
                }
            }

            catch (Exception)
            {
                throw new InternalServerException("An error occured when delete schema");
            }
        }

        public async Task UpdateDatabase()
        {
            _tenantIdentifier.SetCurrentTenantName("public");

            var migrationHistoryScript = "CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\r\n    \"MigrationId\" character varying(150) NOT NULL,\r\n    \"ProductVersion\" character varying(32) NOT NULL,\r\n    CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY (\"MigrationId\")\r\n);";
            var migrationHistoryTablesScript = "SET search_path TO \"$user\", public;\n" +
                "CREATE TABLE IF NOT EXISTS \"public\".\"__SchemaBasedEFMigrationsHistory\" (\r\n    \"MigrationId\" character varying(150) NOT NULL,\r\n    \"SchemaId\" character varying(32) NOT NULL,\r\n    CONSTRAINT \"PK___SchemaBasedEFMigrationsHistory\" PRIMARY KEY (\"MigrationId\", \"SchemaId\")\r\n);" +
                migrationHistoryScript;
            await _dbConnection.ExecuteAsync(migrationHistoryTablesScript);


            using (var db = new FlowDashDbContext(_tenantIdentifier, _configuration))
            {
                string script = string.Empty;
                var pendingMigrations = db.Database.GetPendingMigrations();
                var appliedMigrations = db.Database.GetAppliedMigrations();

                if (pendingMigrations.Any())
                {
                    var migrator = db.GetService<IMigrator>();
                    script = migrator.GenerateScript(fromMigration: appliedMigrations.Count() > 0 ? appliedMigrations.Last() : null, toMigration: pendingMigrations.Last());
                    script = script.Replace(migrationHistoryScript, "");
                    Console.WriteLine(script);
                }

                if (String.IsNullOrEmpty(script.Trim()))
                {
                    return;
                }

                var schemas = (await _dbConnection.QueryAsync<string>(
                    "SELECT schema_name FROM information_schema.schemata"))
                    .Where(w => w != "pg_catalog" && w != "public" && w != "information_schema" && !w.StartsWith("pg_") && w != "hangfire")
                    .ToList();

                // Remove insert to migration table from script
                List<string> matchedInsertMigrations = new List<string>();

                string pattern = @"INSERT INTO ""__EFMigrationsHistory"".*?\n.*?;\s*";

                MatchCollection matches = Regex.Matches(script, pattern, RegexOptions.Singleline);

                foreach (Match match in matches)
                {
                    if (!matchedInsertMigrations.Contains(match.Value))
                    {
                        matchedInsertMigrations.Add(match.Value.Trim());
                    }
                }

                script = Regex.Replace(script, pattern, "", RegexOptions.Singleline);


                // apply sanitized script to all schemas
                foreach (var schema in schemas)
                {
                    var migrations = string.Join(", ", pendingMigrations.Select(s => $"'{s}'").ToList());
                    var existingMigration = (await _dbConnection.QueryAsync<string>(
                        $"SELECT \"MigrationId\" FROM public.\"__SchemaBasedEFMigrationsHistory\" where \"MigrationId\" in ({migrations}) and \"SchemaId\" = '{schema}'"))
                        .FirstOrDefault();

                    if (existingMigration != null)
                    {
                        continue;
                    }

                    string scriptToExecute = $"SET search_path TO {schema};\n" + script;

                    await _dbConnection.ExecuteAsync(scriptToExecute);

                    foreach (var migration in pendingMigrations)
                    {
                        await _dbConnection.ExecuteAsync(
                             $"INSERT INTO public.\"__SchemaBasedEFMigrationsHistory\" (\"MigrationId\",  \"SchemaId\") VALUES ('{migration}', '{schema}')");
                    }
                }

                if (matchedInsertMigrations.Count() > 0)
                {
                    var scriptToExecute = $"SET search_path TO \"$user\", public;\n" + String.Join("\n", matchedInsertMigrations);
                    await _dbConnection.ExecuteAsync(scriptToExecute);
                }
            }
        }
    }
}

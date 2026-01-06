using FlowDash.Application.Common.Interfaces.Service;
using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskModel = FlowDash.Domain.Task;

namespace FlowDash.Infrastructure
{
    public partial class FlowDashDbContext : DbContext
    {
        private readonly ITenantIdentifierService _tenantIdentifier;
        private readonly IConfiguration _configuration;

        public FlowDashDbContext(ITenantIdentifierService tenantIdentifier, IConfiguration configuration)
        {
            _tenantIdentifier = tenantIdentifier;
            _configuration = configuration;
        }
        public FlowDashDbContext(DbContextOptions<FlowDashDbContext> options, ITenantIdentifierService tenantIdentifier, IConfiguration configuration) : base(options)
        {
            _tenantIdentifier = tenantIdentifier;
            _configuration = configuration;
        }

        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<TaskModel> Tasks => Set<TaskModel>();
        public DbSet<TaskActivityLog> TaskActivityLogs => Set<TaskActivityLog>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        public DbSet<Team> Teams => Set<Team>();
        public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            optionsBuilder.UseNpgsql(_configuration.GetConnectionString("DefaultConnection") + $"SearchPath={_tenantIdentifier.GetCurrentTenantName()}",
            options =>
            {
                options.CommandTimeout(30); // Timeout in seconds
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var schema = ResolveSchemaSafely();

            if (!string.IsNullOrWhiteSpace(schema))
            {
                modelBuilder.HasDefaultSchema(schema);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowDashDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        private string? ResolveSchemaSafely()
        {
            // During migrations / design-time
            if (_tenantIdentifier == null)
                return "public";

            var tenant = _tenantIdentifier.GetCurrentTenantName();

            return string.IsNullOrWhiteSpace(tenant) ? "public" : tenant;
        }
    }
}

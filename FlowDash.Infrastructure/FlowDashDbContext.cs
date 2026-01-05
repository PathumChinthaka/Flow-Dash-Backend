using FlowDash.Domain;
using TaskModel = FlowDash.Domain.Task;
using Microsoft.EntityFrameworkCore;

namespace FlowDash.Infrastructure
{
    public class FlowDashDbContext : DbContext
    {
        public FlowDashDbContext(DbContextOptions<FlowDashDbContext> options) : base(options)
        {
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowDashDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

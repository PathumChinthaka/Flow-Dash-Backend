using Microsoft.EntityFrameworkCore;

namespace FlowDash.Infrastructure
{
    public partial class FlowDashDbContext : DbContext
    {
        public FlowDashDbContext(DbContextOptions<FlowDashDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(FlowDashDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

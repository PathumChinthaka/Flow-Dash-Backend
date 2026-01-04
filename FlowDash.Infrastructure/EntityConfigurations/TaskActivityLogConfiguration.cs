using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class TaskActivityLogConfiguration : IEntityTypeConfiguration<TaskActivityLog>
    {
        public void Configure(EntityTypeBuilder<TaskActivityLog> builder)
        {
            builder.HasKey(tal => tal.Id);

            builder.HasOne(tal => tal.Task)
                   .WithMany(t => t.TaskActivityLogs)
                   .HasForeignKey(tal => tal.TaskId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tal => tal.ChangedBy)
                   .WithMany(u => u.TaskChangedBy)
                   .HasForeignKey(tal => tal.ChangedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(tal => tal.Action).IsRequired();

            builder.HasIndex(tal => tal.TaskId);
            builder.HasIndex(tal => tal.ChangedById);
        }
    }
}

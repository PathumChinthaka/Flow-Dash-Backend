using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<FlowDash.Domain.Task>
    {
        public void Configure(EntityTypeBuilder<FlowDash.Domain.Task> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(e => e.CreatedById).HasColumnName("CreatedBy");
            builder.Property(e => e.UpdatedById).HasColumnName("UpdatedBy");

            builder.Property(t => t.Title)
                   .IsRequired()
                   .HasMaxLength(250);

            builder.Property(t => t.Description)
                   .HasMaxLength(2000);

            builder.Property(t => t.Status)
                   .IsRequired();

            builder.HasOne(t => t.Project)
                   .WithMany(p => p.Tasks)
                   .HasForeignKey(t => t.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Assignee)
                   .WithMany(u => u.TaskAssignee)
                   .HasForeignKey(t => t.AssigneeId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(t => t.CreatedBy)
                   .WithMany(u => u.TaskCreatedBy)
                   .HasForeignKey(t => t.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.UpdatedBy)
                   .WithMany(u => u.TaskUpdatedBy)
                   .HasForeignKey(t => t.UpdatedById)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasMany(t => t.TaskActivityLogs)
                   .WithOne(tal => tal.Task)
                   .HasForeignKey(tal => tal.TaskId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(t => t.TaskComments)
                   .WithOne(tc => tc.Task)
                   .HasForeignKey(tc => tc.TaskId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.ProjectId);
            builder.HasIndex(t => t.AssigneeId);
            builder.HasIndex(t => t.CreatedById);
            builder.HasIndex(t => t.UpdatedById);
        }
    }
}

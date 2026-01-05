using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class TaskCommentConfiguration : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.HasKey(tc => tc.Id);
            builder.Property(e => e.CreatedById).HasColumnName("CreatedBy");

            builder.HasOne(tc => tc.Task)
                   .WithMany(t => t.TaskComments)
                   .HasForeignKey(tc => tc.TaskId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(tc => tc.CreatedBy)
                   .WithMany(u => u.TaskCommentCreatedBy)
                   .HasForeignKey(tc => tc.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(tc => tc.Comment).IsRequired();

            builder.HasIndex(tc => tc.TaskId);
            builder.HasIndex(tc => tc.CreatedById);
        }
    }
}

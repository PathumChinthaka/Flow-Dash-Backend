using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(e => e.Id).HasName("Project_pkey");
            builder.ToTable("Project");

            builder.Property(e => e.Id).UseIdentityAlwaysColumn();
            builder.Property(e => e.Guid).HasColumnType("uuid");
            builder.HasKey(p => p.Id);
            builder.Property(e => e.CreatedById).HasColumnName("CreatedBy");
            builder.Property(e => e.UpdatedById).HasColumnName("UpdatedBy");

            builder.HasOne(p => p.CreatedBy)
                   .WithMany(u => u.ProjectCreatedBy)
                   .HasForeignKey(p => p.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.UpdatedBy)
                   .WithMany(u => u.ProjectUpdatedBy)
                   .HasForeignKey(p => p.UpdatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(p => p.Name).IsRequired();

            builder.HasIndex(p => p.CreatedById);
            builder.HasIndex(p => p.UpdatedById);
        }
    }
}

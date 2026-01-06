using FlowDash.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlowDash.Infrastructure.EntityConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.HasKey(e => e.Id).HasName("UserRole_pkey");
            builder.ToTable("UserRole");

            builder.Property(e => e.Id).UseIdentityAlwaysColumn();
            builder.Property(e => e.Guid).HasColumnType("uuid");
            builder.Property(e => e.CreatedById).HasColumnName("CreatedBy");
            builder.Property(e => e.UpdatedById).HasColumnName("UpdatedBy");

            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles)
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles)
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.CreatedBy)
                   .WithMany(u => u.UserRolesCreatedBy)
                   .HasForeignKey(ur => ur.CreatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ur => ur.UpdatedBy)
                   .WithMany(u => u.UserRolesUpdatedBy)
                   .HasForeignKey(ur => ur.UpdatedById)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(ur => ur.CreatedById);
            builder.HasIndex(ur => ur.UpdatedById);
        }
    }
}

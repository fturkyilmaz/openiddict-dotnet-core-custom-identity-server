using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShoppingProject.Core.UserAggregate;

namespace ShoppingProject.Infrastructure.Data.Config;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.Property(r => r.Id)
            .IsRequired();

        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId);
    }
}

using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PasswordSalt).IsRequired().HasMaxLength(256);
        builder.Property(x => x.PasswordIterations).IsRequired();
        builder.Property(x => x.Role).IsRequired().HasMaxLength(64);
    }
}
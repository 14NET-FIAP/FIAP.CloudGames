using FIAP.CloudGames.Domain.Entities;
using FIAP.CloudGames.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.CloudGames.DataBase.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(256);

        builder.HasIndex(u => u.Email)
               .IsUnique();

        // Objeto anônimo porque User só tem setters privados e o construtor público força TypeUser = User.
        builder.HasData(new
        {
            AdminUserSeed.Id,
            AdminUserSeed.Name,
            AdminUserSeed.Email,
            AdminUserSeed.PasswordHash,
            TypeUser = TypeUserEnum.Admin,
            AdminUserSeed.CreatedAt,
            IsActive = true
        });
    }
}

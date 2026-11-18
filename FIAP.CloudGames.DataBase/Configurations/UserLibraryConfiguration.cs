using FIAP.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.CloudGames.DataBase.Configurations;

public class UserLibraryConfiguration : IEntityTypeConfiguration<UserLibrary>
{
    public void Configure(EntityTypeBuilder<UserLibrary> builder)
    {
        // Chave primária composta
        builder.HasKey(ul => new { ul.UserId, ul.GameId });

        builder.HasOne(ul => ul.User)
               .WithMany(u => u.UserLibraries)
               .HasForeignKey(ul => ul.UserId);

        builder.HasOne(ul => ul.Game)
               .WithMany(g => g.UserLibraries)
               .HasForeignKey(ul => ul.GameId);
    }
}

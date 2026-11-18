using FIAP.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FIAP.CloudGames.DataBase.Configurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DiscountPercentage)
               .HasColumnType("decimal(5,2)");

        builder.HasOne(p => p.Game)
               .WithMany(g => g.Promotions)
               .HasForeignKey(p => p.GameId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
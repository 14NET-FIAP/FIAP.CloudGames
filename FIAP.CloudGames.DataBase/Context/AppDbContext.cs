using FIAP.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace FIAP.CloudGames.DataBase.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Game => Set<Game>();
    public DbSet<Promotion> Promotion => Set<Promotion>();



    //protected override void OnModelCreating(ModelBuilder modelBuilder)
    //{
    //    modelBuilder.Entity<User>(e =>
    //    {
    //        e.ToCollection("Users");
    //        e.HasKey(u => u.Id);
    //    });


    //}
}

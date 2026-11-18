using FIAP.CloudGames.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FIAP.CloudGames.DataBase.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Game> Game => Set<Game>();
    public DbSet<Promotion> Promotion => Set<Promotion>();
    public DbSet<UserLibrary> UserLibrary => Set<UserLibrary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
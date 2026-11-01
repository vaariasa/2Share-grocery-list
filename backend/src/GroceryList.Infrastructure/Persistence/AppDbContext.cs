using GroceryList.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryList.Infrastructure.Persistence;

/// <summary>
/// Entity Framework Core context: the bridge between the C# entities and the SQL Server tables.
/// The tables are created by database/create-tables.sql; this mapping must match that script.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<GroceryItem> GroceryItems => Set<GroceryItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(user =>
        {
            user.ToTable("Users");
            user.HasKey(u => u.Id);
            user.Property(u => u.Username).HasMaxLength(50).IsRequired();
            user.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            user.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
            user.HasIndex(u => u.Username).IsUnique();
        });

        modelBuilder.Entity<GroceryItem>(item =>
        {
            item.ToTable("GroceryItems");
            item.HasKey(i => i.Id);
            item.Property(i => i.Name).HasMaxLength(100).IsRequired();
            item.HasOne<User>()
                .WithMany()
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

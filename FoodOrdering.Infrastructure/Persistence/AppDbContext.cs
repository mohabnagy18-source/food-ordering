using FoodOrdering.Application.Abstractions;
using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IApplicationDbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(u => u.Email).IsUnique();
        b.Entity<User>().Property(u => u.Email).HasMaxLength(256);
        b.Entity<User>().Property(u => u.Name).HasMaxLength(120);

        b.Entity<Restaurant>().Property(r => r.Name).HasMaxLength(160);
        b.Entity<Restaurant>().Property(r => r.DeliveryFee).HasPrecision(18, 2);
        b.Entity<MenuItem>().Property(m => m.Price).HasPrecision(18, 2);

        b.Entity<Order>().Property(o => o.Subtotal).HasPrecision(18, 2);
        b.Entity<Order>().Property(o => o.DeliveryFee).HasPrecision(18, 2);
        b.Entity<Order>().Property(o => o.Total).HasPrecision(18, 2);
        b.Entity<Order>().Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        b.Entity<OrderItem>().Property(i => i.UnitPrice).HasPrecision(18, 2);
    }
}

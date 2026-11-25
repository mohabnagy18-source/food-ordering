using FoodOrdering.Domain.Entities;

namespace FoodOrdering.Application.Abstractions;

public interface IApplicationDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<User> Users { get; }
    Microsoft.EntityFrameworkCore.DbSet<Restaurant> Restaurants { get; }
    Microsoft.EntityFrameworkCore.DbSet<MenuItem> MenuItems { get; }
    Microsoft.EntityFrameworkCore.DbSet<Order> Orders { get; }
    Microsoft.EntityFrameworkCore.DbSet<OrderItem> OrderItems { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

public interface IJwtTokenService
{
    string CreateToken(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}

using FoodOrdering.Application.Abstractions;
using FoodOrdering.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Services;

public class CatalogService(IApplicationDbContext db)
{
    public async Task<List<RestaurantDto>> GetRestaurantsAsync(string? search, CancellationToken ct = default)
    {
        var q = db.Restaurants.Include(r => r.MenuItems).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim().ToLower();
            q = q.Where(r => r.Name.ToLower().Contains(s) || r.Cuisine.ToLower().Contains(s));
        }
        var list = await q.OrderByDescending(r => r.Rating).ToListAsync(ct);
        return list.Select(Map).ToList();
    }

    public async Task<RestaurantDto?> GetRestaurantAsync(Guid id, CancellationToken ct = default)
    {
        var r = await db.Restaurants.Include(x => x.MenuItems).FirstOrDefaultAsync(x => x.Id == id, ct);
        return r is null ? null : Map(r);
    }

    private static RestaurantDto Map(Domain.Entities.Restaurant r) => new(
        r.Id, r.Name, r.Cuisine, r.Description, r.Emoji, r.Rating, r.DeliveryMinutes, r.DeliveryFee, r.IsOpen,
        r.MenuItems.Where(m => m.IsAvailable)
            .Select(m => new MenuItemDto(m.Id, m.Name, m.Description, m.Emoji, m.Price, m.Category)).ToList());
}

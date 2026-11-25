using FoodOrdering.Application.Abstractions;
using FoodOrdering.Application.DTOs;
using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Services;

public class OrderService(IApplicationDbContext db)
{
    public async Task<OrderDto> CreateAsync(Guid userId, CreateOrderRequest req, CancellationToken ct = default)
    {
        var restaurant = await db.Restaurants.Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == req.RestaurantId, ct)
            ?? throw new KeyNotFoundException("Restaurant not found.");
        if (!restaurant.IsOpen) throw new InvalidOperationException("This restaurant is currently closed.");
        if (req.Items.Count == 0) throw new ArgumentException("The order must contain at least one item.");
        if (string.IsNullOrWhiteSpace(req.DeliveryAddress)) throw new ArgumentException("A delivery address is required.");

        var order = new Order
        {
            UserId = userId,
            RestaurantId = restaurant.Id,
            DeliveryAddress = req.DeliveryAddress.Trim(),
            Notes = req.Notes,
            DeliveryFee = restaurant.DeliveryFee
        };

        foreach (var line in req.Items)
        {
            if (line.Quantity < 1) continue;
            var item = restaurant.MenuItems.FirstOrDefault(m => m.Id == line.MenuItemId && m.IsAvailable)
                ?? throw new KeyNotFoundException("A menu item in this order is no longer available.");
            order.Items.Add(new OrderItem
            {
                MenuItemId = item.Id,
                MenuItemName = item.Name,
                UnitPrice = item.Price,
                Quantity = line.Quantity
            });
        }
        if (order.Items.Count == 0) throw new ArgumentException("The order must contain at least one item.");

        order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Quantity);
        order.Total = order.Subtotal + order.DeliveryFee;

        db.Orders.Add(order);
        await db.SaveChangesAsync(ct);
        order.Restaurant = restaurant;
        return OrderDto.From(order);
    }

    public async Task<List<OrderDto>> GetMineAsync(Guid userId, CancellationToken ct = default)
    {
        var orders = await db.Orders.Include(o => o.Items).Include(o => o.Restaurant)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(ct);
        return orders.Select(OrderDto.From).ToList();
    }

    public async Task<OrderDto> AdvanceStatusAsync(Guid userId, Guid orderId, CancellationToken ct = default)
    {
        var order = await db.Orders.Include(o => o.Items).Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId, ct)
            ?? throw new KeyNotFoundException("Order not found.");
        if (order.Status < OrderStatus.Delivered)
        {
            order.Status += 1;
            order.UpdatedAt = DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
        }
        return OrderDto.From(order);
    }
}

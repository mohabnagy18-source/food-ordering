using FoodOrdering.Domain.Entities;

namespace FoodOrdering.Application.DTOs;

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record AuthResponse(string Token, string Name, string Email);

public record MenuItemDto(Guid Id, string Name, string Description, string Emoji, decimal Price, string Category);
public record RestaurantDto(Guid Id, string Name, string Cuisine, string Description, string Emoji,
    double Rating, int DeliveryMinutes, decimal DeliveryFee, bool IsOpen, List<MenuItemDto> Menu);

public record OrderItemRequest(Guid MenuItemId, int Quantity);
public record CreateOrderRequest(Guid RestaurantId, string DeliveryAddress, string? Notes, List<OrderItemRequest> Items);

public record OrderItemDto(Guid Id, string Name, decimal UnitPrice, int Quantity);
public record OrderDto(Guid Id, string RestaurantName, string Status, decimal Subtotal, decimal DeliveryFee,
    decimal Total, string DeliveryAddress, string? Notes, DateTime CreatedAt, List<OrderItemDto> Items)
{
    public static OrderDto From(Order o) => new(
        o.Id,
        o.Restaurant?.Name ?? "",
        o.Status.ToString(),
        o.Subtotal, o.DeliveryFee, o.Total,
        o.DeliveryAddress, o.Notes, o.CreatedAt,
        o.Items.Select(i => new OrderItemDto(i.Id, i.MenuItemName, i.UnitPrice, i.Quantity)).ToList());
}

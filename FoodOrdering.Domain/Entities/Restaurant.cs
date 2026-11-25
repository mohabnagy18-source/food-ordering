namespace FoodOrdering.Domain.Entities;

public class Restaurant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Cuisine { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🍽️";
    public double Rating { get; set; }
    public int DeliveryMinutes { get; set; } = 30;
    public decimal DeliveryFee { get; set; }
    public bool IsOpen { get; set; } = true;

    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}

public class MenuItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RestaurantId { get; set; }
    public Restaurant? Restaurant { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🍕";
    public decimal Price { get; set; }
    public string Category { get; set; } = "Main";
    public bool IsAvailable { get; set; } = true;
}

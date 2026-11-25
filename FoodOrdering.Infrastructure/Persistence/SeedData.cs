using FoodOrdering.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        await db.Database.EnsureCreatedAsync();
        if (await db.Restaurants.AnyAsync()) return;

        var r1 = new Restaurant { Name = "Pizza Palace", Cuisine = "Italian", Emoji = "🍕", Rating = 4.7, DeliveryMinutes = 25, DeliveryFee = 2.99m, Description = "Wood-fired pizzas and fresh pasta made daily." };
        r1.MenuItems = new List<MenuItem>
        {
            new() { Name = "Margherita", Description = "Tomato, mozzarella, fresh basil", Emoji = "🍕", Price = 9.99m, Category = "Pizza" },
            new() { Name = "Pepperoni Feast", Description = "Double pepperoni, extra cheese", Emoji = "🍕", Price = 12.49m, Category = "Pizza" },
            new() { Name = "Quattro Formaggi", Description = "Four cheese blend", Emoji = "🧀", Price = 13.99m, Category = "Pizza" },
            new() { Name = "Spaghetti Bolognese", Description = "Slow-cooked beef ragù", Emoji = "🍝", Price = 11.49m, Category = "Pasta" },
            new() { Name = "Tiramisu", Description = "Classic coffee dessert", Emoji = "🍰", Price = 5.99m, Category = "Dessert" }
        };

        var r2 = new Restaurant { Name = "Burger Barn", Cuisine = "American", Emoji = "🍔", Rating = 4.5, DeliveryMinutes = 20, DeliveryFee = 1.99m, Description = "Smashed burgers, crispy fries and thick shakes." };
        r2.MenuItems = new List<MenuItem>
        {
            new() { Name = "Classic Smash", Description = "Beef, cheddar, pickles, house sauce", Emoji = "🍔", Price = 8.99m, Category = "Burgers" },
            new() { Name = "Double Trouble", Description = "Two patties, bacon, jalapeños", Emoji = "🍔", Price = 11.99m, Category = "Burgers" },
            new() { Name = "Crispy Chicken", Description = "Buttermilk chicken, slaw, honey mayo", Emoji = "🍗", Price = 9.49m, Category = "Burgers" },
            new() { Name = "Loaded Fries", Description = "Cheese sauce, bacon bits, spring onion", Emoji = "🍟", Price = 5.49m, Category = "Sides" },
            new() { Name = "Vanilla Shake", Description = "Hand-spun with real ice cream", Emoji = "🥤", Price = 4.49m, Category = "Drinks" }
        };

        var r3 = new Restaurant { Name = "Sushi Zen", Cuisine = "Japanese", Emoji = "🍣", Rating = 4.8, DeliveryMinutes = 35, DeliveryFee = 3.49m, Description = "Fresh sushi rolls and poke bowls." };
        r3.MenuItems = new List<MenuItem>
        {
            new() { Name = "Salmon Nigiri (6pc)", Description = "Fresh Atlantic salmon", Emoji = "🍣", Price = 10.99m, Category = "Sushi" },
            new() { Name = "California Roll (8pc)", Description = "Crab, avocado, cucumber", Emoji = "🍱", Price = 8.99m, Category = "Sushi" },
            new() { Name = "Dragon Roll (8pc)", Description = "Eel, avocado, tobiko", Emoji = "🐉", Price = 13.49m, Category = "Sushi" },
            new() { Name = "Poke Bowl", Description = "Tuna, edamame, rice, ponzu", Emoji = "🥗", Price = 11.99m, Category = "Bowls" },
            new() { Name = "Miso Soup", Description = "Tofu, wakame, spring onion", Emoji = "🍲", Price = 3.49m, Category = "Sides" }
        };

        var r4 = new Restaurant { Name = "Taco Loco", Cuisine = "Mexican", Emoji = "🌮", Rating = 4.4, DeliveryMinutes = 30, DeliveryFee = 2.49m, Description = "Street tacos, burritos and fresh guacamole." };
        r4.MenuItems = new List<MenuItem>
        {
            new() { Name = "Al Pastor Tacos (3)", Description = "Marinated pork, pineapple, coriander", Emoji = "🌮", Price = 8.49m, Category = "Tacos" },
            new() { Name = "Beef Burrito", Description = "Slow-braised beef, rice, beans", Emoji = "🌯", Price = 10.49m, Category = "Burritos" },
            new() { Name = "Quesadilla", Description = "Three cheese, jalapeño, sour cream", Emoji = "🧀", Price = 7.99m, Category = "Mains" },
            new() { Name = "Guac & Chips", Description = "Made-to-order guacamole", Emoji = "🥑", Price = 5.49m, Category = "Sides" },
            new() { Name = "Churros", Description = "Cinnamon sugar, chocolate dip", Emoji = "🍩", Price = 4.99m, Category = "Dessert" }
        };

        var r5 = new Restaurant { Name = "Curry House", Cuisine = "Indian", Emoji = "🍛", Rating = 4.6, DeliveryMinutes = 40, DeliveryFee = 2.99m, Description = "North Indian curries and fresh naan." };
        r5.MenuItems = new List<MenuItem>
        {
            new() { Name = "Butter Chicken", Description = "Creamy tomato, fenugreek", Emoji = "🍛", Price = 12.99m, Category = "Curries" },
            new() { Name = "Lamb Rogan Josh", Description = "Kashmiri chilli, slow-cooked lamb", Emoji = "🍖", Price = 14.49m, Category = "Curries" },
            new() { Name = "Vegetable Biryani", Description = "Saffron rice, raita", Emoji = "🍚", Price = 10.99m, Category = "Rice" },
            new() { Name = "Garlic Naan", Description = "Buttered, fresh coriander", Emoji = "🫓", Price = 3.49m, Category = "Sides" },
            new() { Name = "Mango Lassi", Description = "Alphonso mango, yogurt", Emoji = "🥭", Price = 3.99m, Category = "Drinks" }
        };

        var r6 = new Restaurant { Name = "Green Bowl", Cuisine = "Healthy", Emoji = "🥗", Rating = 4.3, DeliveryMinutes = 25, DeliveryFee = 0.99m, Description = "Salads, smoothie bowls and cold-pressed juice." };
        r6.MenuItems = new List<MenuItem>
        {
            new() { Name = "Caesar Salad", Description = "Grilled chicken, parmesan, croutons", Emoji = "🥗", Price = 9.49m, Category = "Salads" },
            new() { Name = "Buddha Bowl", Description = "Quinoa, roast veg, tahini", Emoji = "🥙", Price = 10.99m, Category = "Bowls" },
            new() { Name = "Acai Bowl", Description = "Acai, granola, berries, honey", Emoji = "🫐", Price = 8.99m, Category = "Bowls" },
            new() { Name = "Green Detox Juice", Description = "Kale, apple, ginger, lemon", Emoji = "🧃", Price = 4.99m, Category = "Drinks" }
        };

        db.Restaurants.AddRange(r1, r2, r3, r4, r5, r6);
        await db.SaveChangesAsync();
    }
}

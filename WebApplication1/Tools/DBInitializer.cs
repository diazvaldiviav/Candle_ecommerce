// Data/DbInitializer.cs
using Candle_API.Data.Entities;

public static class DbInitializer
{
    public static void Initialize(CandleDbContext context)
    {
        context.Database.EnsureCreated();

        // Verificar si ya hay datos
        if (context.Categories.Any())
        {
            return;   // La base de datos ya tiene datos
        }

        // Datos semilla para Categories
        var categories = new Category[]
        {
            new Category
            {
                Name = "Velas Aromáticas",
                Description = "Velas con diferentes aromas y esencias",
                ImageUrl = "https://www.google.com",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Velas Decorativas",
                Description = "Velas con diseños especiales para decoración",
                ImageUrl =  "https://www.google.com",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Velas de Temporada",
                Description = "Velas especiales para festividades y ocasiones",
                ImageUrl =  "https://www.google.com",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var category in categories)
        {
            context.Categories.Add(category);
        }
        context.SaveChanges();

        // Datos semilla para Subcategories
        var subcategories = new SubCategory[]
        {
            new SubCategory
            {
                Name = "Velas de Lavanda",
                Description = "Velas con aroma a lavanda",
                CategoryId = categories[0].Id,
                ImageUrl = "https://www.google.com",
                CreatedAt = DateTime.UtcNow
            },
            new SubCategory
            {
                Name = "Velas de Vainilla",
                Description = "Velas con aroma a vainilla",
                CategoryId = categories[0].Id,
                ImageUrl = "https://www.google.com",
                CreatedAt = DateTime.UtcNow
            },
            new SubCategory
            {
                Name = "Velas Cilíndricas",
                Description = "Velas decorativas de forma cilíndrica",
                CategoryId = categories[1].Id,
                ImageUrl = "https://www.google.com",
                CreatedAt = DateTime.UtcNow
            }
        };

        foreach (var subcategory in subcategories)
        {
            context.Subcategories.Add(subcategory);
        }
        context.SaveChanges();
    }
}
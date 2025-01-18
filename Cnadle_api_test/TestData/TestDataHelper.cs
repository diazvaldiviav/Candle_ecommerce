// TestData/TestDataHelper.cs
using Candle_API.Data.Entities;


namespace Candle_API_test.TestData.TestDataHelper;

public static class TestDataHelper
{
    public static List<Product> GetTestProducts()
    {
        return new List<Product>
        {
            new Product
            {
                Id = 3,
                Name = "Test Product 1",
                Description = "Description 1",
                Price = 19.99m,
                CreatedAt = DateTime.UtcNow,
                SubCategory = new SubCategory { Id = 1, Name = "Test SubCategory" },
                ProductColors = new List<ProductColor>
                {
                    new ProductColor
                    {
                        Id = 1,
                        Color = new Color { Id = 1, Name = "Red" }
                    }
                },
                ProductSizes = new List<ProductSize>
                {
                    new ProductSize
                    {
                        Id = 1,
                        Size = new Size { Id = 1, Name = "M" }
                    }
                }
            }
        };
    }

    public static List<Color> GetTestColors()
    {
        return new List<Color>
        {
            new Color
            {
                Id = 1,
                Name = "Red",
                HexCode = "#FF0000"
            },
            new Color
            {
                Id = 2,
                Name = "Blue",
                HexCode = "#0000FF",
            }
        };
    }

    public static List<Size> GetTestSizes()
    {
        return new List<Size>
        {
            new Size
            {
                Id = 1,
                Name = "Small"
            },
            new Size
            {
                Id = 2,
                Name = "Medium"
            }
        };
    }
}
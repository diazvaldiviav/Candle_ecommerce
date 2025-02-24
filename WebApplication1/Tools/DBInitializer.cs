// Data/DbInitializer.cs
using Candle_API.Data.Entities;

public static class DbInitializer
{
    public static void Initialize(CandleDbContext context)
    {
        context.Database.EnsureCreated();

        // Verificar si ya hay datos
        if (!context.Categories.Any())
        {
            // Datos semilla para Categories
            var categories = new Category[]
            {
            new Category
            {
                Name = "Velas Aromáticas",
                Description = "Velas con diferentes aromas y esencias",
                ImageUrl = "https://images.unsplash.com/photo-1603006905003-be475563bc59",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Velas Decorativas",
                Description = "Velas con diseños especiales para decoración",
                ImageUrl =  "https://images.unsplash.com/photo-1601479604588-68d9e6d8b320",
                CreatedAt = DateTime.UtcNow
            },
            new Category
            {
                Name = "Velas de Temporada",
                Description = "Velas especiales para festividades y ocasiones",
                ImageUrl =  "https://images.unsplash.com/photo-1602874801007-bd36197911b8",
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
                ImageUrl = "https://images.unsplash.com/photo-1602874801007-bd36197911b8",
                CreatedAt = DateTime.UtcNow
            },
            new SubCategory
            {
                Name = "Velas de Vainilla",
                Description = "Velas con aroma a vainilla",
                CategoryId = categories[0].Id,
                ImageUrl = "https://images.unsplash.com/photo-1602874801007-bd36197911b8",
                CreatedAt = DateTime.UtcNow
            },
            new SubCategory
            {
                Name = "Velas Cilíndricas",
                Description = "Velas decorativas de forma cilíndrica",
                CategoryId = categories[1].Id,
                ImageUrl = "https://images.unsplash.com/photo-1602874801007-bd36197911b8",
                CreatedAt = DateTime.UtcNow
            }
            };

            foreach (var subcategory in subcategories)
            {
                context.Subcategories.Add(subcategory);
            }

            context.SaveChanges();

            // Seed Colors
            var colors = new Color[]
            {
            new Color { Name = "Blanco", HexCode = "#FFFFFF" },
            new Color { Name = "Beige", HexCode = "#F5F5DC" },
            new Color { Name = "Rosa", HexCode = "#FFC0CB" },
            new Color { Name = "Violeta", HexCode = "#8A2BE2" },
            new Color { Name = "Verde Sage", HexCode = "#9DC183" }
            };

            foreach (var color in colors)
            {
                context.Colors.Add(color);
            }

            context.SaveChanges();

            // Seed Sizes
            var sizes = new Size[]
            {
            new Size { Name = "Pequeña" },
            new Size { Name = "Mediana" },
            new Size { Name = "Grande" },
            new Size { Name = "Extra Grande" }
            };

            foreach (var size in sizes)
            {
                context.Sizes.Add(size);
            }

            context.SaveChanges();

            // Asumiendo que ya tienes las categorías y subcategorías creadas
            var subcategoryAromaticas = context.Subcategories
                .FirstOrDefault(s => s.Name == "Velas de Lavanda");
            var subcategoryDecorativas = context.Subcategories
                .FirstOrDefault(s => s.Name == "Velas Cilíndricas");

            if (subcategoryAromaticas == null)
            {
                throw new Exception("No se encontró la subcategoría 'Velas de Lavanda'. Asegúrate de crear las categorías y subcategorías primero.");
            }


            // Seed Products
            var products = new Product[]
            {
            new Product
            {
                Name = "Vela de Lavanda Relajante",
                Description = "Vela aromática de lavanda para relajación y aromaterapia",
                Price = 29.99m,
                Stock = 50,
                SubcategoryId = subcategoryAromaticas.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Vela de Lavanda y Vainilla",
                Description = "Mezcla única de lavanda y vainilla para un aroma suave y relajante",
                Price = 34.99m,
                Stock = 40,
                SubcategoryId = subcategoryAromaticas.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Vela Cilíndrica Decorativa",
                Description = "Vela cilíndrica elegante para decoración de mesa",
                Price = 24.99m,
                Stock = 60,
                SubcategoryId = subcategoryDecorativas.Id,
                CreatedAt = DateTime.UtcNow
            },
            new Product
            {
                Name = "Set de Velas Cilíndricas",
                Description = "Set de 3 velas cilíndricas de diferentes tamaños",
                Price = 49.99m,
                Stock = 30,
                SubcategoryId = subcategoryDecorativas.Id,
                CreatedAt = DateTime.UtcNow
            }
            };

            foreach (var product in products)
            {
                context.Products.Add(product);
            }


            context.SaveChanges();



            // Seed ProductColors
            var productColors = new List<ProductColor>
        {
            new ProductColor
            {
                ProductId = products[0].Id,
                ColorId = colors[2].Id // Rosa
            },
            new ProductColor
            {
                ProductId = products[0].Id,
                ColorId = colors[3].Id // Violeta
            },
            new ProductColor
            {
                ProductId = products[1].Id,
                ColorId = colors[1].Id // Beige
            },
            new ProductColor
            {
                ProductId = products[2].Id,
                ColorId = colors[0].Id // Blanco
            },
            new ProductColor
            {
                ProductId = products[2].Id,
                ColorId = colors[4].Id // Verde Sage
            }
        };

            foreach (var productColor in productColors)
            {
                context.ProductColors.Add(productColor);
            }

            context.SaveChanges();

            // Seed ProductSizes
            var productSizes = new List<ProductSize>
        {
            new ProductSize
            {
                ProductId = products[0].Id,
                SizeId = sizes[0].Id // Pequeña
            },
            new ProductSize
            {
                ProductId = products[0].Id,
                SizeId = sizes[1].Id // Mediana
            },
            new ProductSize
            {
                ProductId = products[1].Id,
                SizeId = sizes[1].Id // Mediana
            },
            new ProductSize
            {
                ProductId = products[1].Id,
                SizeId = sizes[2].Id // Grande
            },
            new ProductSize
            {
                ProductId = products[2].Id,
                SizeId = sizes[0].Id // Pequeña
            },
            new ProductSize
            {
                ProductId = products[2].Id,
                SizeId = sizes[1].Id // Mediana
            },
            new ProductSize
            {
                ProductId = products[2].Id,
                SizeId = sizes[2].Id // Grande
            }
            };

            foreach (var productSize in productSizes)
            {
                context.ProductSizes.Add(productSize);
            }

            context.SaveChanges();

        }




       

      

        // Verificar si ya hay roles y usuarios
        if (!context.Roles.Any())
        {
            // Seed Roles
            var roles = new Role[]
            {
        new Role
        {
            Name = "Admin",
            Description = "Administrator role with full access",
            CreatedAt = DateTime.UtcNow
        },
        new Role
        {
            Name = "User",
            Description = "Standard user role",
            CreatedAt = DateTime.UtcNow
        }
            };

            foreach (var role in roles)
            {
                context.Roles.Add(role);
            }
            context.SaveChanges();

            // Crear usuarios por defecto
            var users = new User[]
            {
        new User
        {
            Email = "admin@candleshop.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            FirstName = "Admin",
            LastName = "User",
            PhoneNumber = "1234567890",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Address = "Admin Street 123",
            City = "Admin City",
            State = "AS",
            ZipCode = "12345"
        },
        new User
        {
            Email = "user@candleshop.com",
            Password = BCrypt.Net.BCrypt.HashPassword("User123!"),
            FirstName = "Normal",
            LastName = "User",
            PhoneNumber = "0987654321",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Address = "User Street 456",
            City = "User City",
            State = "US",
            ZipCode = "67890"
        }
            };

            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();

            // Asignar roles a usuarios
            var userRoles = new UserRole[]
            {
        new UserRole
        {
            UserId = users[0].Id,  // Admin
            RoleId = roles[0].Id,  // Admin role
            AssignedAt = DateTime.UtcNow
        },
        new UserRole
        {
            UserId = users[1].Id,  // Normal user
            RoleId = roles[1].Id,  // User role
            AssignedAt = DateTime.UtcNow
        }
            };

            foreach (var userRole in userRoles)
            {
                context.UserRoles.Add(userRole);
            }
            context.SaveChanges();
        }
    }
}
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Candle_API.Data.DTOs.Product;
using Candle_API.Data.Entities;
using Candle_API.Services.Implementations;
using Candle_API_test.Helpers.TestBase;
using Candle_API_test.TestData.TestDataHelper;
using Candle_api_test.TestData;


namespace Candle_API.Tests.Services
{
    public class ProductServiceTests : TestBase
    {
        private readonly ProductServices _productService;
        private readonly Mock<ILogger<ProductServices>>_mockLogger;
        private readonly Mock<DbSet<Product>> _mockProductDbSet;

        public ProductServiceTests()
        {
            _mockLogger = CreateMockLogger<ProductServices>();

            // Crear datos de prueba
            var testProducts = TestDataHelper.GetTestProducts();
            var queryableTestProducts = testProducts.AsQueryable();

            // Configurar el mock del DbSet
            _mockProductDbSet = new Mock<DbSet<Product>>();



            // Configurar IQueryable
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(queryableTestProducts.Provider));
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.Expression)
                .Returns(queryableTestProducts.Expression);
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.ElementType)
                .Returns(queryableTestProducts.ElementType);
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => queryableTestProducts.GetEnumerator());

            // Configurar el DbContext
            MockContext.Setup(c => c.Set<Product>())
                .Returns(_mockProductDbSet.Object);

            // Crear el servicio
            _productService = new ProductServices(
                MockContext.Object,
                MockMapper.Object,
                _mockLogger.Object
            );
        }



        [Fact]
        public async Task GetProductByIdAsync_ExistingProduct_ReturnsProduct()
        {
            // Arrange
            var testProduct = TestDataHelper.GetTestProducts().First();
            var expectedDto = new ProductDto
            {
                Id = testProduct.Id,
                Name = testProduct.Name,
                Description = testProduct.Description,
                Price = testProduct.Price
            };

            // Configurar el queryable con los datos de prueba
            var testProducts = new List<Product> { testProduct }.AsQueryable();

            // Configurar el mock del DbSet
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(testProducts.Provider));
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.Expression)
                .Returns(testProducts.Expression);
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.ElementType)
                .Returns(testProducts.ElementType);
            _mockProductDbSet.As<IQueryable<Product>>()
                .Setup(m => m.GetEnumerator())
                .Returns(testProducts.GetEnumerator());

            // Configurar el mock del DbContext usando Set<Product>()
            MockContext.Setup(c => c.Set<Product>())
                .Returns(_mockProductDbSet.Object);

            // Configurar el mapper
            MockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(expectedDto);

            // Act
            var result = await _productService.GetProductByIdAsync(testProduct.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(testProduct.Id);
            result.Name.Should().Be(testProduct.Name);
            result.Description.Should().Be(testProduct.Description);
            result.Price.Should().Be(testProduct.Price);
        }


        [Fact]
        public async Task GetProductByIdAsync_NonExistingProduct_ThrowsKeyNotFoundException()
        {
            // Arrange
            int nonExistingId = 999;
            var testProducts = new List<Product>().AsQueryable();

            // Configurar el mock del DbSet con una lista vacía
            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(testProducts.Provider));
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.Expression)
                .Returns(testProducts.Expression);
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.ElementType)
                .Returns(testProducts.ElementType);
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.GetEnumerator())
                .Returns(testProducts.GetEnumerator());

            // Configurar el contexto para usar el nuevo mock
            MockContext.Setup(c => c.Set<Product>())
                .Returns(mockSet.Object);

            // Act & Assert
            await _productService.Invoking(s => s.GetProductByIdAsync(nonExistingId))
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"No se encontró el producto con ID: {nonExistingId}");
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsAllProducts()
        {
            // Arrange
            var testProducts = TestDataHelper.GetTestProducts();
            var testProductsQueryable = testProducts.AsQueryable();

            // Configurar el mock del DbSet
            var mockSet = new Mock<DbSet<Product>>();

            // Configurar IQueryable
            mockSet.As<IAsyncEnumerable<Product>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Product>(testProducts.GetEnumerator()));

            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(testProductsQueryable.Provider));
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.Expression)
                .Returns(testProductsQueryable.Expression);
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.ElementType)
                .Returns(testProductsQueryable.ElementType);
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.GetEnumerator())
                .Returns(() => testProductsQueryable.GetEnumerator());

            // Configurar el contexto
            MockContext.Setup(c => c.Set<Product>())
                .Returns(mockSet.Object);

            // Configurar el mapper
            var expectedDtos = testProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                SubcategoryName = p.SubCategory.Name,

            });

            MockMapper.Setup(m => m.Map<IEnumerable<ProductDto>>(It.IsAny<List<Product>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(testProducts.Count);
            result.Should().BeEquivalentTo(expectedDtos);
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_ReturnsCreatedProduct()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Name = "New Product",
                Description = "New Description",
                Price = 29.99m
            };

            var expectedProduct = new Product
            {
                Id = 1,
                Name = createDto.Name,
                Description = createDto.Description,
                Price = createDto.Price
            };

            var expectedDto = new ProductDto
            {
                Id = expectedProduct.Id,
                Name = expectedProduct.Name,
                Description = expectedProduct.Description,
                Price = expectedProduct.Price
            };

            MockMapper.Setup(m => m.Map<Product>(createDto))
                .Returns(expectedProduct);

            MockMapper.Setup(m => m.Map<ProductDto>(expectedProduct))
                .Returns(expectedDto);

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
            MockContext.Verify(m => m.Set<Product>().Add(It.IsAny<Product>()), Times.Once);
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var existingProduct = TestDataHelper.GetTestProducts().First();
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Price = 39.99m
            };

            var expectedDto = new ProductDto
            {
                Id = existingProduct.Id,
                Name = updateDto.Name,
                Description = updateDto.Description,
                Price = updateDto.Price
            };

            _mockProductDbSet.Setup(m => m.FindAsync(existingProduct.Id))
                .ReturnsAsync(existingProduct);

            MockMapper.Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(expectedDto);

            // Act
            var result = await _productService.UpdateProductAsync(existingProduct.Id, updateDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedDto);
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_ReturnsTrue()
        {
            // Arrange
            var existingProduct = TestDataHelper.GetTestProducts().First();

            _mockProductDbSet.Setup(m => m.FindAsync(existingProduct.Id))
                .ReturnsAsync(existingProduct);

            // Act
            var result = await _productService.DeleteProductAsync(existingProduct.Id);

            // Assert
            result.Should().BeTrue();
            MockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ThrowsKeyNotFoundException()
        {
            // Arrange
            int nonExistingId = 999;
            _mockProductDbSet.Setup(m => m.FindAsync(nonExistingId))
                .ReturnsAsync((Product)null);

            // Act & Assert
            await _productService.Invoking(s => s.DeleteProductAsync(nonExistingId))
                .Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage($"No se encontró el producto con ID: {nonExistingId}");
        }
    }
}
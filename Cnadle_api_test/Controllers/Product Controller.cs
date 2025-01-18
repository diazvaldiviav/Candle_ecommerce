using Candle_API.Controllers;
using Candle_API.Data.DTOs.Product;
using Candle_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using Candle_API_test.Helpers.TestBase;

namespace Cnadle_api_test.ControllersTests;

// Controllers/ProductControllerTests.cs
public class ProductControllerTests : TestBase
{
    private readonly ProductsController _controller;
    private readonly Mock<IProduct> _mockProductService;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProduct>();
        _mockLogger = CreateMockLogger<ProductsController>();
        _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProduct_ExistingProduct_ReturnsOkResult()
    {
        // Arrange
        var productId = 1;
        var expectedProduct = new ProductDto { Id = productId, Name = "Test Product" };
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
            .ReturnsAsync(expectedProduct);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProduct = okResult.Value.Should().BeOfType<ProductDto>().Subject;
        returnedProduct.Should().BeEquivalentTo(expectedProduct);
    }

    [Fact]
    public async Task GetProduct_NonExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var productId = 999;
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId))
            .ThrowsAsync(new KeyNotFoundException($"No se encontró el producto con ID: {productId}"));

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        result.Result.Should().BeOfType<NotFoundObjectResult>();
    }
}

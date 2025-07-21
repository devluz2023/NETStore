using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.Models;
using MyApi.Controllers; // Ensure this namespace matches your ProductsController location
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ProductsControllerTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly ProductsController _controller;

    // Constructor runs before each test
    public ProductsControllerTests()
    {
        // Configure in-memory database for testing
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Use unique name for each test
            .Options;

        _context = new MyDbContext(options);
        _controller = new ProductsController(_context);

        // Seed data for tests
        SeedDatabase();
    }

    // Dispose method runs after each test
    public void Dispose()
    {
        // Ensure the database is deleted after each test to maintain isolation
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    private void SeedDatabase()
    {
        // Clear any existing data if necessary (though unique DB name helps)
        _context.Products.RemoveRange(_context.Products);
        _context.SaveChanges();

        _context.Products.AddRange(
            // Seed Product 1 with the "updated" data to match the GET response expectation
            new Product
            {
                Id = 1,
                Title = "Updated Product Title atualizado",
                Price = 150.00m,
                Description = "Updated desWcription",
                Category = "New Category",
                Image = "https://example.com/new-image.jpg",
                Rating = new Rating { Rate = 4.9, Count = 20 }
            },
            new Product { Id = 2, Title = "Product B", Price = 20.00m, Description = "Desc B", Category = "Books", Image = "imgB.jpg", Rating = new Rating { Rate = 3.8, Count = 50 } },
            new Product { Id = 3, Title = "Product C", Price = 15.00m, Description = "Desc C", Category = "Electronics", Image = "imgC.jpg", Rating = new Rating { Rate = 4.0, Count = 75 } },
            new Product { Id = 4, Title = "Product D", Price = 25.00m, Description = "Desc D", Category = "Clothing", Image = "imgD.jpg", Rating = new Rating { Rate = 4.9, Count = 200 } },
            new Product { Id = 5, Title = "Product E", Price = 5.00m, Description = "Desc E", Category = "Books", Image = "imgE.jpg", Rating = new Rating { Rate = 3.0, Count = 20 } }
        );
        _context.SaveChanges();
    }

    // --- GET /products/{id} Test ---

    [Fact]
    public async Task GetProduct_ReturnsProduct_ForValidId()
    {
        // Arrange
        var productId = 1; // We expect to retrieve the product with ID 1 from the seeded data

        // Act
        // Call GetById as per your ProductsController, which returns Task<IActionResult>
        var result = await _controller.GetById(productId);

        // Assert
        // Cast the IActionResult directly to OkObjectResult to access its Value
        var okResult = Assert.IsType<OkObjectResult>(result);
        var product = Assert.IsType<Product>(okResult.Value);

        // Assert properties based on the user's provided GET response for ID 1
        Assert.Equal(1, product.Id);
        Assert.Equal("Updated Product Title atualizado", product.Title);
        Assert.Equal(150.00m, product.Price);
        Assert.Equal("Updated desWcription", product.Description);
        Assert.Equal("New Category", product.Category);
        Assert.Equal("https://example.com/new-image.jpg", product.Image);

        Assert.NotNull(product.Rating);
        Assert.Equal(0, product.Rating.Id); // Owned entity ID is often 0 for in-memory or new entities
        Assert.Equal(4.9, product.Rating.Rate);
        Assert.Equal(20, product.Rating.Count);
        Assert.Null(product.CartId); // Based on your GET response
    }

    [Fact]
    public void GetProducts_ReturnsPaginatedAndOrderedResults()
    {
        // Arrange
        int page = 1;
        int size = 2;
        string order = "price desc,title asc"; // Order by price descending, then title ascending

        // Act
        // Call the Get method directly, as it returns IActionResult (not Task<IActionResult>)
        var result = _controller.Get(page, size, order);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic responseData = okResult.Value; // Use dynamic to easily access properties from anonymous object

        // Assert pagination metadata
        Assert.Equal(5, (int)responseData.totalItems);
        Assert.Equal(page, (int)responseData.currentPage);
        Assert.Equal(3, (int)responseData.totalPages); // 5 items, 2 per page = 3 pages (ceil(5/2))

        // Assert data content and order
        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(responseData.data).ToList();
        Assert.Equal(size, products.Count);

        // Expected order: Product D (25.00), Product B (20.00)
        // Then for ties, by title asc (not applicable in this small set for price desc)
        Assert.Equal("Product D", products[0].Title); // Price 25.00
        Assert.Equal("Product B", products[1].Title); // Price 20.00
    }

    // --- GET /products (Default Pagination) Test ---
    // This test covers retrieving all products with default pagination (no query parameters).
    [Fact]
    public void GetProducts_ReturnsAllProducts_WhenNoPaginationOrOrdering()
    {
        // Act
        var result = _controller.Get(); // Use default page/size

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        dynamic responseData = okResult.Value;

        Assert.Equal(5, (int)responseData.totalItems);
        Assert.Equal(1, (int)responseData.currentPage);
        Assert.Equal(1, (int)responseData.totalPages); // Default size 10, so all fit on one page

        var products = Assert.IsAssignableFrom<IEnumerable<Product>>(responseData.data).ToList();
        Assert.Equal(5, products.Count);
        // Default order is by Id, so Product A (Id 1) should be first
        Assert.Equal("Updated Product Title atualizado", products[0].Title); // Product 1 is now "Updated Product Title atualizado"
    }
}
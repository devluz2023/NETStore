using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly MyDbContext _context;

        public ProductsController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            return Ok(product);
        }


        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (product == null)
                return BadRequest();

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "_page")] int page = 1,
                                     [FromQuery(Name = "_size")] int size = 10,
                                     [FromQuery(Name = "_order")] string order = null)
        {
            var query = _context.Products.AsQueryable();

            // Filtering, sorting
            if (!string.IsNullOrEmpty(order))
            {
                // Example: "price desc, title asc"
                var orders = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var o in orders)
                {
                    var parts = o.Trim().Split(' ');
                    var property = parts[0];
                    var direction = parts.Length > 1 ? parts[1] : "asc";

                    switch (property.ToLower())
                    {
                        case "price":
                            query = direction == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                            break;
                        case "title":
                            query = direction == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
                            break;
                            // add other properties to sort
                    }
                }
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);

            var products = query.Skip((page - 1) * size).Take(size).ToList();

            return Ok(new
            {
                data = products,
                totalItems = totalItems,
                currentPage = page,
                totalPages = totalPages
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null || id != updatedProduct.Id)
                return BadRequest();

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

            // Update fields
            existingProduct.Title = updatedProduct.Title;
            existingProduct.Price = updatedProduct.Price;
            existingProduct.Description = updatedProduct.Description;
            existingProduct.Category = updatedProduct.Category;
            existingProduct.Image = updatedProduct.Image;
            existingProduct.Rating = updatedProduct.Rating;

            await _context.SaveChangesAsync();

            return Ok(existingProduct);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Product deleted successfully" });
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToList();

            return Ok(categories);
        }
        [HttpGet("category/{category}")]
        public IActionResult GetProductsByCategory(
           string category,
           [FromQuery(Name = "_page")] int page = 1,
           [FromQuery(Name = "_size")] int size = 10,
           [FromQuery(Name = "_order")] string order = null)
        {
            var query = _context.Products
                .Where(p => p.Category == category)
                .AsQueryable();

            // Sorting
            if (!string.IsNullOrEmpty(order))
            {
                var orders = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var o in orders)
                {
                    var parts = o.Trim().Split(' ');
                    var property = parts[0];
                    var direction = parts.Length > 1 ? parts[1] : "asc";

                    switch (property.ToLower())
                    {
                        case "price":
                            query = direction == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                            break;
                        case "title":
                            query = direction == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
                            break;
                            // add more properties if needed
                    }
                }
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);

            var products = query
                .Skip((page - 1) * size)
                .Take(size)
                .ToList();

            return Ok(new
            {
                data = products,
                totalItems,
                currentPage = page,
                totalPages
            });
        }
    }
}
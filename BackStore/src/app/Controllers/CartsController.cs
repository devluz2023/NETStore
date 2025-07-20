// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using MyApi.Data;
// using MyApi.Models;
// using System.Globalization;
// using System;
// namespace MyApi.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class CartsController : ControllerBase
//     {
//         private readonly MyDbContext _context;

//         public CartsController(MyDbContext context)
//         {
//             _context = context;
//         }

//         [HttpGet("{id}")]
//         public async Task<IActionResult> Get(int id)
//         {
//             var cart = await _context.Carts
//                 .Include(c => c.Products)
//                 .FirstOrDefaultAsync(c => c.Id == id);

//             if (cart == null)
//                 return NotFound();

//             return Ok(cart);
//         }


//         [HttpPost]
//         public async Task<IActionResult> CreateCart([FromBody] Cart cart)
//         {
//             if (cart == null)
//                 return BadRequest();

//             // Make sure Date is UTC
//             cart.Date = DateTime.SpecifyKind(cart.Date, DateTimeKind.Utc);

//             // Add the new cart to the context
//             _context.Carts.Add(cart);
//             await _context.SaveChangesAsync();

//             return Ok(cart);
//         }

//         [HttpPut("{id}")]
//         public async Task<IActionResult> UpdateCart(int id, [FromBody] Cart updatedCart)
//         {
//             if (updatedCart == null || id != updatedCart.Id)
//                 return BadRequest();

//             // Fetch existing cart including its products
//             var existingCart = await _context.Carts
//                 .Include(c => c.Products)
//                 .FirstOrDefaultAsync(c => c.Id == id);

//             if (existingCart == null)
//                 return NotFound();

//             // Update scalar properties
//             existingCart.UserId = updatedCart.UserId;
//             existingCart.Date = updatedCart.Date;

//             // Handle products collection update
//             // Remove existing products not in the update
//             var toRemove = existingCart.Products.Where(p => !updatedCart.Products.Any(up => up.ProductId == p.ProductId)).ToList();
//             _context.CartProducts.RemoveRange(toRemove);

//             // Update existing or add new Product entries
//             foreach (var updatedProduct in updatedCart.Products)
//             {
//                 var existingProduct = existingCart.Products.FirstOrDefault(p => p.ProductId == updatedProduct.ProductId);
//                 if (existingProduct != null)
//                 {
//                     existingProduct.Quantity = updatedProduct.Quantity;
//                 }
//                 else
//                 {
//                     // Add new CartProduct
//                     existingCart.Products.Add(new CartProduct
//                     {
//                         ProductId = updatedProduct.ProductId,
//                         Quantity = updatedProduct.Quantity,
//                         CartId = existingCart.Id
//                     });
//                 }
//             }

//             await _context.SaveChangesAsync();

//             return Ok(existingCart);
//         }
//         [HttpGet]
//         public IActionResult Get([FromQuery(Name = "_page")] int page = 1,
//                                  [FromQuery(Name = "_size")] int size = 10,
//                                  [FromQuery(Name = "_order")] string order = null)
//         {
//             var query = _context.Carts.AsQueryable();

//             // Sorting implementation
//             if (!string.IsNullOrEmpty(order))
//             {
//                 var orders = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
//                 foreach (var o in orders)
//                 {
//                     var parts = o.Trim().Split(' ');
//                     var property = parts[0];
//                     var direction = parts.Length > 1 ? parts[1] : "asc";

//                     switch (property.ToLower())
//                     {
//                         case "id":
//                             query = direction == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id);
//                             break;
//                         case "userid":
//                             query = direction == "desc" ? query.OrderByDescending(c => c.UserId) : query.OrderBy(c => c.UserId);
//                             break;
//                         case "date":
//                             query = direction == "desc" ? query.OrderByDescending(c => c.Date) : query.OrderBy(c => c.Date);
//                             break;
//                             // Add other properties as needed
//                     }
//                 }
//             }

//             int totalItems = query.Count();
//             int totalPages = (int)Math.Ceiling(totalItems / (double)size);

//             var carts = query
//                 .Skip((page - 1) * size)
//                 .Take(size)
//                 .ToList();

//             return Ok(new
//             {
//                 data = carts,
//                 totalItems,
//                 currentPage = page,
//                 totalPages
//             });
//         }

//     }
// }
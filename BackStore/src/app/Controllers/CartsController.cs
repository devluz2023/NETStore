using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;
using System; // Required for DateTime
using System.Text.Json.Serialization; // Required for [JsonIgnore]

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _context;

        public CartController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            return Ok(cart);
        }



        [HttpPost]
        public async Task<IActionResult> CreateCart([FromBody] Cart cart)
        {
       
            if (cart == null)
            {
                return BadRequest("Cart data is null.");
            }

            if (cart.Date.Kind == DateTimeKind.Unspecified || cart.Date.Kind == DateTimeKind.Local)
            {
                cart.Date = cart.Date.ToUniversalTime();
            }

       
            var productsToProcess = cart.Products.ToList();
            cart.Products.Clear(); 

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();

 
            if (productsToProcess != null && productsToProcess.Any())
            {
                foreach (var product in productsToProcess)
                {
                   
                    if (product.Id > 0)
                    {
                        var existingProduct = await _context.Products.FindAsync(product.Id);
                        if (existingProduct != null)
                        {
                            existingProduct.CartId = cart.Id;
                            _context.Products.Update(existingProduct);
                        }
                        else
                        {
                         
                            product.CartId = cart.Id; 
                            _context.Products.Add(product);
                        }
                    }
                    else 
                    {
                        product.CartId = cart.Id; 
                        _context.Products.Add(product);
                    }
                }
                await _context.SaveChangesAsync(); // Save changes for products
            }

            // After SaveChangesAsync, cart.Id will be populated with the database-generated ID
            // Using nameof(GetCart) as that's the name of the GET method in this controller.
            // Reload the cart with its products to ensure the response includes them
            // as they were processed in a separate SaveChangesAsync call.
            var createdCart = await _context.Carts
                                            .Include(c => c.Products)
                                            .FirstOrDefaultAsync(c => c.Id == cart.Id);

            return CreatedAtAction(nameof(GetById), new { id = createdCart.Id }, createdCart);
        
        }


        // [HttpGet]
        // public IActionResult Get([FromQuery(Name = "_page")] int page = 1,
        //                              [FromQuery(Name = "_size")] int size = 10,
        //                              [FromQuery(Name = "_order")] string order = null)
        // {
        //     var query = _context.Cart.AsQueryable();

        //     // Filtering, sorting
        //     if (!string.IsNullOrEmpty(order))
        //     {
        //         // Example: "price desc, title asc"
        //         var orders = order.Split(',', StringSplitOptions.RemoveEmptyEntries);
        //         foreach (var o in orders)
        //         {
        //             var parts = o.Trim().Split(' ');
        //             var property = parts[0];
        //             var direction = parts.Length > 1 ? parts[1] : "asc";

        //             switch (property.ToLower())
        //             {
        //                 case "price":
        //                     query = direction == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
        //                     break;
        //                 case "title":
        //                     query = direction == "desc" ? query.OrderByDescending(p => p.Title) : query.OrderBy(p => p.Title);
        //                     break;
        //                     // add other properties to sort
        //             }
        //         }
        //     }

        //     var totalItems = query.Count();
        //     var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        //     var carts = query.Skip((page - 1) * size).Take(size).ToList();

        //     return Ok(new
        //     {
        //         data = carts,
        //         totalItems = totalItems,
        //         currentPage = page,
        //         totalPages = totalPages
        //     });
        // }


        // [HttpPut("{id}")]
        // public async Task<IActionResult> UpdateCartt(int id, [FromBody] Cart updatedCartt)
        // {
        //     if (updatedCartt == null || id != updatedCartt.Id)
        //         return BadRequest();

        //     var existingCartt = await _context.Cart.FindAsync(id);
        //     if (existingCartt == null)
        //         return NotFound();

        //     // // Update fields
        //     // existingCartt.Title = updatedCartt.Title;
        //     // existingCartt.Price = updatedCartt.Price;
        //     // existingCartt.Description = updatedCartt.Description;
        //     // existingCartt.Category = updatedCartt.Category;
        //     // existingCartt.Image = updatedCartt.Image;
        //     // existingCartt.Rating = updatedCartt.Rating;

        //     await _context.SaveChangesAsync();

        //     return Ok(existingCartt);
        // }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
                return NotFound();

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cart deleted successfully" });
        }


    }
}
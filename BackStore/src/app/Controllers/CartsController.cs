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


   [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetPaginatedCarts(
            [FromQuery(Name = "_page")] int page = 1,
            [FromQuery(Name = "_size")] int size = 10,
            [FromQuery(Name = "_order")] string order = null)
        {
            // Ensure page and size are positive
            if (page < 1) page = 1;
            if (size < 1) size = 10;

            IQueryable<Cart> query = _context.Carts.Include(c => c.Products); // Include products by default

            // Apply ordering if specified
            if (!string.IsNullOrEmpty(order))
            {
                // Simple ordering based on a property name (e.g., "date", "-date" for descending)
                switch (order.ToLower())
                {
                    case "date":
                        query = query.OrderBy(c => c.Date);
                        break;
                    case "-date":
                        query = query.OrderByDescending(c => c.Date);
                        break;
                    // Add more ordering options as needed, e.g., by cart ID
                    case "id":
                        query = query.OrderBy(c => c.Id);
                        break;
                    case "-id":
                        query = query.OrderByDescending(c => c.Id);
                        break;
                    default:
                        // Default ordering if 'order' is not recognized
                        query = query.OrderBy(c => c.Id);
                        break;
                }
            }
            else
            {
                // Default ordering if no order parameter is provided
                query = query.OrderBy(c => c.Id);
            }

            // Apply pagination
            var carts = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            if (carts == null || !carts.Any())
            {
                return NotFound("No carts found for the given pagination criteria.");
            }

            // You might want to add pagination metadata to the response headers
            // For example: X-Total-Count, X-Total-Pages, etc.
            // This requires getting the total count before applying Skip/Take.
            // var totalCount = await _context.Carts.CountAsync();
            // Response.Headers.Add("X-Total-Count", totalCount.ToString());

            return Ok(carts);
        }
        

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCart(int id, [FromBody] Cart updatedCart)
        {
            // 1. Validate input
            if (updatedCart == null || id != updatedCart.Id)
            {
                return BadRequest("Cart ID mismatch or invalid cart data.");
            }

            // 2. Find the existing cart in the database
            var existingCart = await _context.Carts
                                             .Include(c => c.Products) // Eagerly load products for update logic
                                             .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCart == null)
            {
                return NotFound($"Cart with ID {id} not found.");
            }

            // 3. Update scalar properties of the existing cart
            // IMPORTANT: Ensure the DateTime Kind is UTC before saving to PostgreSQL
            if (updatedCart.Date.Kind == DateTimeKind.Unspecified || updatedCart.Date.Kind == DateTimeKind.Local)
            {
                existingCart.Date = updatedCart.Date.ToUniversalTime();
            }
            else
            {
                existingCart.Date = updatedCart.Date;
            }

            // 4. Handle product collection updates
            // Get current product IDs in the cart
            var currentProductIds = existingCart.Products.Select(p => p.Id).ToList();
            // Get incoming product IDs from the updated cart payload
            var incomingProductIds = updatedCart.Products.Select(p => p.Id).ToList();

            // Products to remove from the cart (those in current but not in incoming)
            var productsToRemoveIds = currentProductIds.Except(incomingProductIds).ToList();
            foreach (var productIdToRemove in productsToRemoveIds)
            {
                var productToRemove = existingCart.Products.FirstOrDefault(p => p.Id == productIdToRemove);
                if (productToRemove != null)
                {
                    productToRemove.CartId = null; // Disassociate product from this cart
                    _context.Products.Update(productToRemove);
                }
            }

            // Products to add/associate with the cart (those in incoming but not in current)
            foreach (var incomingProduct in updatedCart.Products)
            {
                if (!currentProductIds.Contains(incomingProduct.Id))
                {
                    // This product is new to this cart
                    var productToAssociate = await _context.Products.FindAsync(incomingProduct.Id);
                    if (productToAssociate != null)
                    {
                        // Associate existing product
                        productToAssociate.CartId = existingCart.Id;
                        _context.Products.Update(productToAssociate);
                    }
                    else
                    {
                        // If product doesn't exist, you might want to create it or return an error.
                        // For now, if ID is 0, we assume it's a new product to be created.
                        // If ID > 0 and not found, it's an invalid ID, handle as appropriate.
                        if (incomingProduct.Id == 0)
                        {
                            incomingProduct.CartId = existingCart.Id;
                            _context.Products.Add(incomingProduct);
                        }
                        else
                        {
                            // Log or return error for invalid product ID in payload
                            return BadRequest($"Product with ID {incomingProduct.Id} not found and cannot be associated.");
                        }
                    }
                }
                // If the product already exists in the cart, no action is needed here
                // unless you have other properties on the Product that need updating
                // from the payload (e.g., if Product was an owned entity or a complex type).
                // For this one-to-many, we're only managing the CartId.
            }

            // Mark the cart itself as modified (for its scalar properties like Date)
            _context.Entry(existingCart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw; // Re-throw if it's a different concurrency issue
                }
            }

            // Reload the updated cart with its products to ensure the response includes them
            var resultCart = await _context.Carts
                                          .Include(c => c.Products)
                                          .FirstOrDefaultAsync(c => c.Id == id);

            return Ok(resultCart); // Return the updated cart
        }

        // Helper method to check if a cart exists (for concurrency handling)
        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.Id == id);
        }

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
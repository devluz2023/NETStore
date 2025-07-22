// MyApi.Controllers/CartsController.cs
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using MongoDB.Driver.Linq; 


using System.Linq; 
using System.Threading.Tasks;


namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartsController : ControllerBase
    {
        private readonly MongoDbContext _mongoContext;

        public CartsController(MongoDbContext mongoContext)
        {
            _mongoContext = mongoContext;
        }

      
        [HttpGet]
        public async Task<IActionResult> GetCarts([FromQuery] int _page = 1, [FromQuery] int _size = 10, [FromQuery] string _order = null)
        {
            var queryable = _mongoContext.Carts.AsQueryable();

            // Sorting
            if (!string.IsNullOrEmpty(_order))
            {
                var orderParts = _order.Split(',');
                foreach (var part in orderParts)
                {
                    var fieldAndDirection = part.Trim().Split(' ');
                    var field = fieldAndDirection[0].ToLower();
                    var direction = fieldAndDirection.Length > 1 && fieldAndDirection[1].ToLower() == "desc" ? -1 : 1;

                    switch (field)
                    {
                        case "id":
                            queryable = direction == -1 ? queryable.OrderByDescending(c => c.Id) : queryable.OrderBy(c => c.Id);
                            break;
                        case "userid":
                            queryable = direction == -1 ? queryable.OrderByDescending(c => c.UserId) : queryable.OrderBy(c => c.UserId);
                            break;
                        case "date":
                            queryable = direction == -1 ? queryable.OrderByDescending(c => c.Date) : queryable.OrderBy(c => c.Date);
                            break;
                        default:
                            // Optionally handle unknown fields or apply a default sort
                            break;
                    }
                }
            }
            else
            {
                queryable = queryable.OrderBy(c => c.Id); 
            }

            var totalItems = await _mongoContext.Carts.CountDocumentsAsync(_ => true);
            var totalPages = (int)Math.Ceiling(totalItems / (double)_size);


            var carts = await queryable
                                .Skip((_page - 1) * _size)
                                .Take(_size) 
                                .ToListAsync();


            return Ok(new
            {
                data = carts,
                totalItems,
                currentPage = _page,
                totalPages
            });
        }

   
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var cart = await _mongoContext.Carts.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (cart == null)
                return NotFound(new { message = "Cart not found" });
            return Ok(cart);
        }

  
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCartDto input)
        {
            var cart = new Cart
            {
                UserId = input.UserId,
                Date = input.Date.ToUniversalTime(),
                Products = input.Products.Select(p => new CartProduct
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity
                }).ToList()
            };

            await _mongoContext.Carts.InsertOneAsync(cart);

            return CreatedAtAction(nameof(GetById), new { id = cart.Id }, cart);
        }

      
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateCartDto input)
        {
            var existingCart = await _mongoContext.Carts.Find(c => c.Id == id).FirstOrDefaultAsync();
            if (existingCart == null)
                return NotFound(new { message = "Cart not found" });

            existingCart.UserId = input.UserId;
            existingCart.Date = input.Date.ToUniversalTime(); 

       
            existingCart.Products.Clear(); 
            existingCart.Products.AddRange(input.Products.Select(p => new CartProduct
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }));

            var result = await _mongoContext.Carts.ReplaceOneAsync(c => c.Id == id, existingCart);

            if (result.ModifiedCount == 0)
            {
                return StatusCode(500, new { message = "Failed to update cart or no changes were made." });
            }

            return Ok(existingCart);
        }

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mongoContext.Carts.DeleteOneAsync(c => c.Id == id);

            if (result.DeletedCount == 0)
                return NotFound(new { message = "Cart not found" });

            return Ok(new { message = "Cart deleted successfully" });
        }
    }
}

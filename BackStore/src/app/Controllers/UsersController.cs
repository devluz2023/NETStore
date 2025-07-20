using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApi.Data;
using MyApi.Models;

namespace MyApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _context;

        public UsersController(MyDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        [HttpGet]
        public IActionResult Get([FromQuery(Name = "_page")] int page = 1,
                                     [FromQuery(Name = "_size")] int size = 10,
                                     [FromQuery(Name = "_order")] string order = null)
        {
            var query = _context.Users.AsQueryable();

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
                        case "id":
                            query = direction == "desc" ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id);
                            break;
                        case "username":
                            query = direction == "desc" ? query.OrderByDescending(u => u.Username) : query.OrderBy(u => u.Username);
                            break;
                        case "email":
                            query = direction == "desc" ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                            break;
                            // add other properties to sort
                    }
                }
            }

            var totalItems = query.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)size);

            var users = query.Skip((page - 1) * size).Take(size).ToList();

            return Ok(new
            {data = users,
                totalItems,
                currentPage = page,
                totalPages
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (updatedUser == null || id != updatedUser.Id)
                return BadRequest();

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            existingUser.Email = updatedUser.Email;
    existingUser.Username = updatedUser.Username;
    existingUser.Password = updatedUser.Password;
    existingUser.Name.Firstname = updatedUser.Name.Firstname;
    existingUser.Name.Lastname = updatedUser.Name.Lastname;
    existingUser.Address.City = updatedUser.Address.City;
    existingUser.Address.Street = updatedUser.Address.Street;
    existingUser.Address.Number = updatedUser.Address.Number;
    existingUser.Address.Zipcode = updatedUser.Address.Zipcode;
    existingUser.Address.Geolocation.Lat = updatedUser.Address.Geolocation.Lat;
    existingUser.Address.Geolocation.Long = updatedUser.Address.Geolocation.Long;
    existingUser.Phone = updatedUser.Phone;
    existingUser.Status = updatedUser.Status;
    // existingUser.Role = updatedUser.Role;


            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return Ok(new { message = "User deleted successfully" });
        }

    
    }
}
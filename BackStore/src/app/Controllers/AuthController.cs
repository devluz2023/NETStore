using Microsoft.AspNetCore.Mvc;
using MyApi.Data;
using MyApi.Models;
using MyApi.Services;
using Microsoft.EntityFrameworkCore; // For FindAsync and other EF Core methods
using System.Threading.Tasks; // For async/await

namespace MyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthController(MyDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Username and password are required.");
            }

            // Find the user by username
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            // IMPORTANT: In a real application, you would hash and salt passwords.
            // For demonstration, we'll do a simple comparison.
            // You should use a library like BCrypt.Net-Core for password hashing.
            // Example: if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            if (user.Password != request.Password) // This is a placeholder for actual password verification
            {
                return Unauthorized("Invalid credentials.");
            }

            // Generate JWT token
            var token = _tokenService.GenerateToken(user);

            return Ok(new AuthResponse { Token = token, Username = user.Username });
        }
    }
}
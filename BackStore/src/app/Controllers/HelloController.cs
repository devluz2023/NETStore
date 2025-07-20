using Microsoft.AspNetCore.Authorization; // Required for [Authorize]
using Microsoft.AspNetCore.Mvc;

namespace HelloWorldApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HelloController : ControllerBase
    {
        // This endpoint will require authentication because of the [Authorize] attribute.
        // Users must provide a valid JWT token to access this.
        [HttpGet("protected")] // Changed route to be more explicit for protected endpoint
        [Authorize]
        public string GetProtected()
        {
            return "Hello World 2 - This is a PROTECTED endpoint!";
        }

        // This endpoint will NOT require authentication.
        // Anyone can access this without a JWT token.
        [HttpGet("public")] // Changed route to be more explicit for public endpoint
        public string GetPublic()
        {
            return "Hello World 2 - This is a PUBLIC endpoint!";
        }
    }
}

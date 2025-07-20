using MyApi.Models; // Required for User model

namespace MyApi.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}

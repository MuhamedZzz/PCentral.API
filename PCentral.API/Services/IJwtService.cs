using PCentral.API.Models;

namespace PCentral.API.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
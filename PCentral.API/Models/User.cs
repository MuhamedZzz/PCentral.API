using Microsoft.AspNetCore.Identity;

namespace PCentral.API.Models
{
    public class User : IdentityUser<int>
    {
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
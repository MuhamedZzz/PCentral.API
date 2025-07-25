namespace PCentral.API.DTOs
{
    public class AuthResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
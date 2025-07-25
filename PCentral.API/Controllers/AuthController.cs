using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PCentral.API.DTOs;
using PCentral.API.Models;
using PCentral.API.Services;

namespace PCentral.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _users;
        private readonly SignInManager<User> _signIn;
        private readonly IJwtService _jwt;

        public AuthController(UserManager<User> users, SignInManager<User> signIn, IJwtService jwt)
        {
            _users = users;
            _signIn = signIn;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Bio = null,
                AvatarUrl = null
            };

            var createResult = await _users.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
                return BadRequest(createResult.Errors);

            var token = _jwt.GenerateToken(user);
            var response = new AuthResponseDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Token = token
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            // First, get the user by email to get their username
            var user = await _users.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            // Use the username for sign-in (since UserName = Email in your case, this is the same)
            var signInResult = await _signIn.PasswordSignInAsync(user.UserName!, dto.Password, false, false);
            if (!signInResult.Succeeded)
                return Unauthorized("Invalid email or password.");

            var token = _jwt.GenerateToken(user);
            var response = new AuthResponseDto
            {
                Id = user.Id,
                UserName = user.UserName!,
                Email = user.Email!,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl,
                Token = token
            };

            return Ok(response);
        }
    }
}
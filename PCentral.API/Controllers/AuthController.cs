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
            var user = new User { UserName = dto.Email, Email = dto.Email };
            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return BadRequest(res.Errors);
            var token = _jwt.GenerateToken(user);
            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var res = await _signIn.PasswordSignInAsync(dto.Email, dto.Password, false, false);
            if (!res.Succeeded) return Unauthorized();
            var user = await _users.FindByEmailAsync(dto.Email);
            var token = _jwt.GenerateToken(user!);
            return Ok(new { token });
        }
    }
}
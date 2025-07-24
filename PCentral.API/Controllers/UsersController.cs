using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PCentral.API.DTOs;
using PCentral.API.Models;
using System.Security.Claims;

namespace PCentral.API.Controllers
{
    [ApiController]
    [Route("api/users/me")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _users;
        public UsersController(UserManager<User> users)
        {
            _users = users;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _users.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            return Ok(new
            {
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl
            });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateProfileDto dto)
        {
            var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _users.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            user.Bio = dto.Bio;
            user.AvatarUrl = dto.AvatarUrl;

            var result = await _users.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return NoContent();
        }
    }
}
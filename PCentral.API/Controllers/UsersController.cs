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
                Id = user.Id,
                Username = user.UserName,
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
            if (user == null)
                return NotFound();

            // 1) If they passed a new username, attempt to change it
            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.UserName)
            {
                // This will check uniqueness, update normalized name, etc.
                var usernameResult = await _users.SetUserNameAsync(user, dto.Username);
                if (!usernameResult.Succeeded)
                    // return all Identity errors (e.g. duplicate username)
                    return BadRequest(usernameResult.Errors);
            }

            // 2) Update the rest of their profile
            user.Bio = dto.Bio;
            user.AvatarUrl = dto.AvatarUrl;

            var updateResult = await _users.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            // 3) (optional) return the updated resource
            return Ok(new
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Bio = user.Bio,
                AvatarUrl = user.AvatarUrl
            });
        }
    }
}
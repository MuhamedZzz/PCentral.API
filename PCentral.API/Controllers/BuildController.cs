using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PCentral.API.Data;
using PCentral.API.DTOs;
using PCentral.API.Models;
using System.Security.Claims;

namespace PCentral.API.Controllers
{
    [ApiController]
    [Route("api/build")]
    [Authorize]
    public class BuildController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public BuildController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var build = await _db.Builds.FirstOrDefaultAsync(b => b.UserId == userId);

            if (build == null)
                return NotFound();

            var parts = JsonSerializer.Deserialize<List<string>>(build.PartsJson)
                        ?? new List<string>();

            return Ok(new BuildDto { Parts = parts });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] BuildDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var json = JsonSerializer.Serialize(dto.Parts);

            var existing = await _db.Builds.FirstOrDefaultAsync(b => b.UserId == userId);
            if (existing != null)
            {
                existing.PartsJson = json;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                _db.Builds.Add(new Build
                {
                    UserId = userId,
                    PartsJson = json
                });
            }

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var build = await _db.Builds.FirstOrDefaultAsync(b => b.UserId == userId);

            if (build != null)
            {
                _db.Builds.Remove(build);
                await _db.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}

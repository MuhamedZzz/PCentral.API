// Controllers/StaticPartsController.cs
using Microsoft.AspNetCore.Mvc;
using PCentral.API.Services;
using PCentral.API.DTOs;
using PCentral.API.Models;

namespace PCentral.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaticPartsController : ControllerBase
    {
        private readonly IStaticPartService _staticPartService;
        private readonly ILogger<StaticPartsController> _logger;

        public StaticPartsController(IStaticPartService staticPartService, ILogger<StaticPartsController> logger)
        {
            _staticPartService = staticPartService;
            _logger = logger;
        }

        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _staticPartService.GetAvailableCategories();
            return Ok(categories);
        }

        [HttpGet("{category}")]
        public IActionResult GetByCategory(string category)
        {
            try
            {
                var parts = _staticPartService.GetAll(category);

                if (!parts.Any())
                {
                    return NotFound($"No parts found for category: {category}");
                }

                var dto = new StaticPartListDto
                {
                    Category = category,
                    Count = parts.Count(),
                    Parts = parts.Select(MapToDto).ToList()
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting parts for category: {Category}", category);
                return StatusCode(500, "An error occurred while retrieving parts");
            }
        }

        [HttpGet("{category}/{id}")]
        public IActionResult GetById(string category, string id)
        {
            try
            {
                var part = _staticPartService.GetById(category, id);

                if (part == null)
                {
                    return NotFound($"Part with ID {id} not found in category {category}");
                }

                var dto = MapToDto(part);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting part {Id} from category {Category}", id, category);
                return StatusCode(500, "An error occurred while retrieving the part");
            }
        }

        private static StaticPartDto MapToDto(StaticPart part)
        {
            return new StaticPartDto
            {
                Id = part.Id,
                Name = part.Name,
                Category = part.Category,
                Price = part.Price,
                Properties = part.Properties
            };
        }
    }
}
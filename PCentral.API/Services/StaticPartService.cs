using PCentral.API.Models;
using System.Text.Json;

namespace PCentral.API.Services
{
    public interface IStaticPartService
    {
        IEnumerable<StaticPart> GetAll(string category);
        StaticPart? GetById(string category, string id);
        IEnumerable<string> GetAvailableCategories();
    }

    public class StaticPartService : IStaticPartService
    {
        private readonly Dictionary<string, List<StaticPart>> _store;
        private readonly ILogger<StaticPartService> _logger;

        public StaticPartService(IWebHostEnvironment env, ILogger<StaticPartService> logger)
        {
            _logger = logger;
            _store = new Dictionary<string, List<StaticPart>>();
            LoadData(env);
        }

        private void LoadData(IWebHostEnvironment env)
        {
            try
            {
                var dataFolder = Path.Combine(env.ContentRootPath, "Data", "StaticParts");

                if (!Directory.Exists(dataFolder))
                {
                    _logger.LogWarning("StaticParts folder not found at: {DataFolder}", dataFolder);
                    return;
                }

                var jsonFiles = Directory.GetFiles(dataFolder, "*.json");
                _logger.LogInformation("Found {FileCount} JSON files in {DataFolder}", jsonFiles.Length, dataFolder);

                foreach (var filePath in jsonFiles)
                {
                    try
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath);
                        var category = fileName.Replace("-", "").ToLower();

                        var jsonContent = File.ReadAllText(filePath);
                        var jsonElements = JsonSerializer.Deserialize<JsonElement[]>(jsonContent);

                        var parts = new List<StaticPart>();

                        for (int i = 0; i < jsonElements.Length; i++)
                        {
                            var element = jsonElements[i];
                            var part = new StaticPart
                            {
                                Id = $"{category}_{i + 1}",
                                Category = category,
                                Properties = new Dictionary<string, object>()
                            };

                            if (element.TryGetProperty("name", out var nameElement))
                            {
                                part.Name = nameElement.GetString() ?? "";
                            }

                            if (element.TryGetProperty("price", out var priceElement))
                            {
                                if (priceElement.ValueKind == JsonValueKind.Number)
                                {
                                    part.Price = priceElement.GetDecimal();
                                }
                            }

                            foreach (var property in element.EnumerateObject())
                            {
                                part.Properties[property.Name] = ConvertJsonElement(property.Value);
                            }

                            parts.Add(part);
                        }

                        _store[category] = parts;
                        _logger.LogInformation("Loaded {PartCount} parts for category: {Category}", parts.Count, category);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error loading file: {FilePath}", filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading static parts data");
            }
        }

        private object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? "",
                JsonValueKind.Number => element.TryGetInt32(out var intVal) ? intVal : element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null,
                JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToArray(),
                JsonValueKind.Object => element.EnumerateObject()
                    .ToDictionary(prop => prop.Name, prop => ConvertJsonElement(prop.Value)),
                _ => element.ToString()
            };
        }

        public IEnumerable<StaticPart> GetAll(string category)
        {
            var normalizedCategory = category.Replace("-", "").ToLower();
            return _store.TryGetValue(normalizedCategory, out var list) ? list : Enumerable.Empty<StaticPart>();
        }

        public StaticPart? GetById(string category, string id)
        {
            var normalizedCategory = category.Replace("-", "").ToLower();
            return _store.TryGetValue(normalizedCategory, out var list)
                ? list.FirstOrDefault(p => p.Id == id)
                : null;
        }

        public IEnumerable<string> GetAvailableCategories()
        {
            return _store.Keys;
        }
    }
}
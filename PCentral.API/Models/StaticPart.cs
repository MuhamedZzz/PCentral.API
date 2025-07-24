using System.Text.Json;

namespace PCentral.API.Models
{
    public class StaticPart
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }
}
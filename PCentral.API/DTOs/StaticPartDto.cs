namespace PCentral.API.DTOs
{
    public class StaticPartDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class StaticPartListDto
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
        public List<StaticPartDto> Parts { get; set; } = new();
    }
}
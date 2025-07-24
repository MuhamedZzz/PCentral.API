namespace PCentral.API.Models
{
    public class Build
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Reuse this field to store JSON-serialized Parts
        public string PartsJson { get; set; } = "[]";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

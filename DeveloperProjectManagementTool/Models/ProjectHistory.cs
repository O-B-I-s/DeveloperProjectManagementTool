namespace DeveloperProjectManagementTool.Models
{
    public class ProjectHistory
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Action { get; set; }
        public string Details { get; set; }
    }
}

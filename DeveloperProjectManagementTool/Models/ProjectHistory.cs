using DeveloperProjectManagementTool.Areas.Identity;

namespace DeveloperProjectManagementTool.Models
{
    public class ProjectHistory
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Action { get; set; }
        public string Details { get; set; }
        public int? ProjectId { get; set; }
        public int? SprintId { get; set; }
        public int? IssueId { get; set; }
        public int? SubTaskId { get; set; }
        public string UserId { get; set; }

        // Navigation properties
        public Project Project { get; set; }
        public Sprint Sprint { get; set; }
        public Issue Issue { get; set; }
        public SubTask SubTask { get; set; }
        public ApplicationUser User { get; set; }
    }
}

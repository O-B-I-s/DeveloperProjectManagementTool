using Microsoft.AspNetCore.Identity;

namespace DeveloperProjectManagementTool.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
        public string ReporterId { get; set; }
        public IdentityUser Reporter { get; set; }
        public PriorityLevel Priority { get; set; }
        public IssueType Type { get; set; }
        public IssueStatus Status { get; set; } = IssueStatus.TODO;
        public List<SubTask> SubTasks { get; set; } = new();
    }

    public enum PriorityLevel { Low, Medium, High }
    public enum IssueType { Epic, Task, Bug, Story }
    public enum IssueStatus { TODO, InProgress, Done }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeveloperProjectManagementTool.Models
{
    public class Issue
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Attachment { get; set; }
        public string? ReporterId { get; set; }
        public IdentityUser? Reporter { get; set; }
        public PriorityLevel? Priority { get; set; }
        public IssueType Type { get; set; }
        public IssueStatus Status { get; set; } = IssueStatus.TODO;
        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; }
        public int? SprintId { get; set; }
        [ForeignKey("SprintId")]
        public Sprint Sprint { get; set; }
        public List<SubTask> SubTask { get; set; } = new();
    }

    public enum PriorityLevel { Low, Medium, High }
    public enum IssueType { Epic, Task, Bug, Story }
    public enum IssueStatus { TODO, InProgress, Done }
}

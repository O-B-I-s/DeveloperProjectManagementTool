using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeveloperProjectManagementTool.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        [Required]
        public int IssueId { get; set; }
        [ForeignKey("IssueId")]
        public Issue? Issue { get; set; }
        public TasksStatus Status { get; set; } = TasksStatus.TODO;
    }
    public enum TasksStatus { TODO, InProgress, Done }
}

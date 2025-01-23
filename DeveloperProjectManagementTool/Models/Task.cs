using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeveloperProjectManagementTool.Models
{
    public class Task
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
        public Issue Issue { get; set; }
    }
}

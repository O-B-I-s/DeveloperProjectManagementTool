using System.ComponentModel.DataAnnotations;

namespace DeveloperProjectManagementTool.Models
{
    public class Sprint
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DurationInDays { get; set; }
        [Required]
        public SprintStatus Status { get; set; } = SprintStatus.NotStarted;
        public List<Issue> Issues { get; set; } = new();

        public void CalculateEndDate()
        {
            EndDate = StartDate.AddDays(DurationInDays);
        }
    }

    public enum SprintStatus { NotStarted, InProgress, Completed }
}

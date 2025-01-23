namespace DeveloperProjectManagementTool.Models
{
    public class SprintIssuesViewModel
    {
        public int SprintId { get; set; }
        public string SprintName { get; set; }
        public List<Issue> AvailableIssues { get; set; } = new();
        public List<int> SelectedIssueIds { get; set; } = new();
    }
}

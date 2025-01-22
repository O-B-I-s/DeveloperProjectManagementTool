namespace DeveloperProjectManagementTool.Models
{
    public class SubTask
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Assignee { get; set; }
        public IssueStatus Status { get; set; } = IssueStatus.TODO;
    }
}

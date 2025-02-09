using DeveloperProjectManagementTool.Areas.Identity;

namespace DeveloperProjectManagementTool.Models
{
    public class ProjectUserRole
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Role { get; set; }
    }
}

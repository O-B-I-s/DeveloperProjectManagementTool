using DeveloperProjectManagementTool.Areas.Identity;

namespace DeveloperProjectManagementTool.Models
{
    public class UserOrganization
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int OrganizationId { get; set; }
        public Organization Organization { get; set; }
    }
}

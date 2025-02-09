using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Identity;

namespace DeveloperProjectManagementTool.Areas.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();

        public ICollection<Project> Projects { get; set; }
        public ICollection<ProjectUserRole> ProjectRoles { get; set; }

    }
}

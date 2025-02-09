using DeveloperProjectManagementTool.Areas.Identity;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeveloperProjectManagementTool.Models
{
    public class Project
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public IdentityUser Owner { get; set; }
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }
        public List<Sprint> Sprints { get; set; } = new();

        public List<ProjectHistory> History { get; set; } = new();
        public List<ApplicationUser> Users { get; internal set; }
        public ICollection<ProjectUserRole> UserRoles { get; set; }
    }
}

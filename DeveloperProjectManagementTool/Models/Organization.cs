using System.ComponentModel.DataAnnotations;

namespace DeveloperProjectManagementTool.Models
{
    public class Organization
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public ICollection<UserOrganization> UserOrganizations { get; set; } = new List<UserOrganization>();
    }
}

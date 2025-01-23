using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectManagementTool.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<ProjectHistory> ProjectHistories { get; set; }
        public DbSet<Models.Task> Tasks { get; set; } = default!;
    }
}

using DeveloperProjectManagementTool.Areas.Identity;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ProjectUserRole> ProjectUserRoles { get; set; }
        public DbSet<UserOrganization> UserOrganizations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // Set string length for Identity Keys (avoids clustered index issues)
            builder.Entity<ApplicationUser>()
                .Property(u => u.Id)
                .HasMaxLength(450);


            // Define SubTasks and Users relationship
            builder.Entity<SubTask>()
                .HasOne(st => st.AssignedUser)
                .WithMany()
                .HasForeignKey(st => st.AssignedUserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete


            //// Define Organization and User relationship
            //builder.Entity<Organization>()
            //    .HasMany(o => o.Users)
            //    .WithOne(u => u.Organization)
            //    .HasForeignKey(u => u.OrganizationId)
            //    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Define Project and User many-to-many relationship
            builder.Entity<Project>()
                .HasMany(p => p.Users)
                .WithMany(u => u.Projects)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjectUsers", // Custom table name
                    j => j.HasOne<ApplicationUser>()
                          .WithMany()
                          .HasForeignKey("UserId")
                          .OnDelete(DeleteBehavior.Restrict), // Avoid cascade paths
                    j => j.HasOne<Project>()
                          .WithMany()
                          .HasForeignKey("ProjectId")
                          .OnDelete(DeleteBehavior.Restrict)) // Avoid cascade paths
                .ToTable("ProjectUsers"); // Ensure table name is consistent
            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

                // Adjust Name column configuration
                entity.Property(t => t.Name)
                    .HasMaxLength(200); // Example
            });
            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(l => new { l.LoginProvider, l.ProviderKey });

                entity.Property(l => l.ProviderKey)
                    .HasMaxLength(200); // Update column definition
            });
            //builder.Entity<ApplicationUser>()
            //        .HasOne(u => u.Organization)
            //        .WithMany(o => o.Users)
            //        .HasForeignKey(u => u.OrganizationId)
            //        .OnDelete(DeleteBehavior.SetNull); // Set foreign key to NULL



            // Define User-Project-Role relationship
            builder.Entity<ProjectUserRole>()
     .HasKey(pur => pur.Id); // Use single primary key

            builder.Entity<ProjectUserRole>()
                .HasIndex(pur => new { pur.ProjectId, pur.UserId }) // Add unique index
                .IsUnique();

            builder.Entity<ProjectUserRole>()
                .HasOne(pur => pur.Project)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(pur => pur.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProjectUserRole>()
                .HasOne(pur => pur.User)
                .WithMany()
                .HasForeignKey(pur => pur.UserId)
                .OnDelete(DeleteBehavior.Restrict);




        }

    }
}

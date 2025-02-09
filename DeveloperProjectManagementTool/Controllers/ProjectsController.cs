using DeveloperProjectManagementTool.Areas.Identity;
using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DeveloperProjectManagementTool.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HistoryLoggerService _historyLogger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(ApplicationDbContext context, HistoryLoggerService historyLogger, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _historyLogger = historyLogger;
            _userManager = userManager;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var projects = _context.Projects
               .Include(p => p.Owner)
               .Include(s => s.Sprints)
               .Include(o => o.Organization);
                return View(await projects.ToListAsync());
            }
            var userId = _userManager.GetUserId(User); // Get logged-in user's ID

            // Get the logged-in user
            var user = await _context.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return Unauthorized();
            }

            // Get the organizations the user belongs to
            var userOrganizationIds = _context.UserOrganizations
                .Where(uo => uo.UserId == userId)
                .Select(uo => uo.OrganizationId)
                .ToList();

            // Fetch projects where the user is the owner or in the same organization
            var applicationDbContext = _context.Projects
                .Include(p => p.Owner)
                .Include(s => s.Sprints)
                .Include(o => o.Organization)
                .Where(p => p.OwnerId == userId || userOrganizationIds.Contains(p.OrganizationId));
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = _userManager.GetUserId(User); // Get logged-in user's ID

            // Get the organizations the user belongs to
            var userOrganizationIds = await _context.UserOrganizations
                .Where(uo => uo.UserId == userId)
                .Select(uo => uo.OrganizationId)
                .ToListAsync();

            // Fetch the organizations the user belongs to
            var organizations = await _context.Organizations
                .Where(o => userOrganizationIds.Contains(o.Id))
                .ToListAsync();

            ViewData["OrganizationId"] = new SelectList(organizations, "Id", "Name");

            // Set the OwnerId to the logged-in user
            var currentUser = await _userManager.GetUserAsync(User);
            ViewData["OwnerId"] = new SelectList(new[] { currentUser }, "Id", "UserName");

            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,OwnerId,OrganizationId")] Models.Project project)
        {


            if (ModelState.IsValid)
            {



                // If the OwnerId is not set (which it shouldn't be), set it to the logged-in user
                if (project.OwnerId == null)
                {
                    project.OwnerId = _userManager.GetUserId(User); // Assign the logged-in user as the owner
                }

                var organization = await _context.Organizations
                    .FirstOrDefaultAsync(o => o.Id == project.OrganizationId);

                if (organization == null)
                {
                    ModelState.AddModelError("", "Invalid organization selected.");

                    return View(project);
                }

                var users = await _context.UserOrganizations
                    .Where(uo => uo.OrganizationId == project.OrganizationId)
                    .Select(uo => uo.User)
                    .ToListAsync();

                project.Users = new List<ApplicationUser>();

                _context.Add(project);
                await _context.SaveChangesAsync();
                // Log history
                await _historyLogger.LogHistory(
                    "Created",
                    $"Project '{project.Name}' was Created.",
                    projectId: project.Id
                );


                // Assign Team Leader role to the creator
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get logged-in user ID
                var projectUserRole = new ProjectUserRole
                {
                    ProjectId = project.Id,
                    UserId = userId,
                    Role = "Team Leader"
                };
                _context.Add(projectUserRole);
                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            var userID = _userManager.GetUserId(User); // Get logged-in user's ID

            // Get the organizations the user belongs to
            var userOrganizationIds = await _context.UserOrganizations
                .Where(uo => uo.UserId == userID)
                .Select(uo => uo.OrganizationId)
                .ToListAsync();

            // Fetch the organizations the user belongs to
            var organizations = await _context.Organizations
                .Where(o => userOrganizationIds.Contains(o.Id))
                .ToListAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            ViewData["OrganizationId"] = new SelectList(organizations, "Id", "Name");

            ViewData["OwnerId"] = new SelectList(new[] { currentUser }, "Id", "UserName");
            return View(project);
        }



        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.UserRoles)  // Ensure UserRoles is included
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // Ensure UserRoles is not null
            project.UserRoles ??= new List<ProjectUserRole>();

            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name", project.OrganizationId);
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "UserName", project.OwnerId);

            return View(project);
        }


        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,OwnerId,OrganizationId")] Models.Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();



                    // Log history
                    await _historyLogger.LogHistory(
                        "Updated",
                        $"Project '{project.Name}' was updated.",
                        projectId: project.Id
                    );
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name");
                ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", project.Owner);
                return RedirectToAction(nameof(Index));
            }

            ViewData["OrganizationId"] = new SelectList(_context.Organizations, "Id", "Name");
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", project.Owner);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects
         .Include(p => p.UserRoles) // Include ProjectUserRoles
         .Include(p => p.Users) // Include Users (Many-to-Many)
         .Include(p => p.History) // Include ProjectHistory
         .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // Remove many-to-many relationships manually
            project.Users.Clear(); // This removes entries from "ProjectUsers"
            _context.ProjectUserRoles.RemoveRange(project.UserRoles); // Remove role mappings
            _context.ProjectHistories.RemoveRange(project.History); // Remove history

            await _context.SaveChangesAsync();

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }
    }
}

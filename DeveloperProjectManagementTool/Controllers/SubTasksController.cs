using DeveloperProjectManagementTool.Areas.Identity;
using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectManagementTool.Controllers
{
    public class SubTasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubTasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: SubTasks
        public async Task<IActionResult> Index(int id)
        {
            var tasks = await _context.SubTasks
                 .Where(t => t.IssueId == id)
                 .Include(t => t.Issue)
                 .Include(a => a.AssignedUser)
                 .ToListAsync();
            foreach (var task in tasks)
            {
                Console.WriteLine($"Task: {task.Title}, Issue: {task.Issue?.Id}, Assigned User: {task.AssignedUser?.UserName}");
            }

            return View(tasks);
        }

        public async Task<IActionResult> AssignedTasks(int id)
        {

            var userId = _userManager.GetUserId(User); // Get logged-in user's ID
            var tasks = await _context.SubTasks
                 .Where(t => t.IssueId == id && t.AssignedUserId == userId)
                 .Include(a => a.AssignedUser)
                 .Include(i => i.Issue)
                 .ToListAsync();


            return View(tasks);
        }

        // GET: SubTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks
                .Include(s => s.Issue)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subTask == null)
            {
                return NotFound();
            }

            return View(subTask);
        }

        // GET: SubTasks/Create
        public IActionResult Create(int issueId)
        {
            var subTask = new SubTask
            {
                IssueId = issueId
            };

            // Get the organization ID
            var organizationId = _context.Issues
                .Where(i => i.Id == issueId)
                .Select(i => i.Sprint.Project.OrganizationId)
                .FirstOrDefault();

            if (organizationId == 0)
            {
                ModelState.AddModelError("", "Unable to determine the organization for the selected issue.");
                return View(subTask);
            }

            // Fetch users belonging to the same organization
            var users = _context.UserOrganizations
                .Where(uo => uo.OrganizationId == organizationId)
                .Select(uo => new
                {
                    uo.User.Id,
                    uo.User.UserName
                })
                .ToList();

            // Debugging: Output the users retrieved
            Console.WriteLine($"Users Count: {users.Count}");
            foreach (var user in users)
            {
                Console.WriteLine($"User: {user.UserName}, UserId: {user.Id}");
            }

            // Populate ViewData for the AssignedUserId dropdown
            ViewData["AssignedUserId"] = new SelectList(users, "Id", "UserName");

            // Populate ViewData for the Status dropdown
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Models.TasksStatus)).Cast<Models.TasksStatus>());

            // Return the view
            return View();
        }


        // POST: SubTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsCompleted,IssueId, AssignedUserId")] SubTask subTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subTask);
                await _context.SaveChangesAsync();
                //ViewData["IssueId"] = subTask.IssueId;
                return RedirectToAction(nameof(AssignedTasks), new { id = subTask.IssueId });
            }
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", subTask.IssueId);
            return View(subTask);
        }

        // GET: SubTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks
                .Include(s => s.AssignedUser)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (subTask == null)
            {
                return NotFound();
            }

            // Populate dropdown for AssignedUser
            ViewData["AssignedUserId"] = new SelectList(_context.Users, "Id", "UserName", subTask.AssignedUserId);
            ViewData["Status"] = new SelectList(Enum.GetValues(typeof(Models.TasksStatus)).Cast<Models.TasksStatus>());
            return View(subTask);
        }

        // POST: SubTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,IssueId,AssignedUserId,Status")] SubTask subTask)
        {
            if (id != subTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubTaskExists(subTask.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(AssignedTasks), new { id = subTask.IssueId });
            }

            // Ensure AssignedUser dropdown is populated
            ViewData["AssignedUserId"] = new SelectList(_context.Users, "Id", "UserName", subTask.AssignedUserId);
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", subTask.IssueId);
            return View(subTask);
        }


        // GET: SubTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks
                .Include(s => s.Issue)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subTask == null)
            {
                return NotFound();
            }

            return View(subTask);
        }

        // POST: SubTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask != null)
            {
                _context.SubTasks.Remove(subTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(AssignedTasks), new { id = subTask.IssueId });
        }

        private bool SubTaskExists(int id)
        {
            return _context.SubTasks.Any(e => e.Id == id);
        }
    }
}

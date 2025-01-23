using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectManagementTool.Controllers
{
    public class SprintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SprintsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sprints
        public async Task<IActionResult> Index()
        {
            return View(await _context.Sprints.ToListAsync());
        }

        // GET: Sprints/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sprint = await _context.Sprints
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sprint == null)
            {
                return NotFound();
            }

            return View(sprint);
        }

        // GET: Sprints/Create
        public IActionResult Create(int projectId)
        {
            var projects = new Project
            {
                Id = projectId,
            };
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(SprintStatus))
           .Cast<SprintStatus>()
           .Select(s => new SelectListItem
           {
               Value = s.ToString(),
               Text = s.ToString()
           }), "Value", "Text");
            return View();
        }

        // POST: Sprints/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,StartDate,EndDate,DurationInDays,Status,ProjectId")] Sprint sprint)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(sprint);
            await _context.SaveChangesAsync();

            //}
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(SprintStatus))
            .Cast<SprintStatus>()
            .Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }), "Value", "Text");
            return RedirectToAction(nameof(Index));
            //return View(sprint);
        }

        // GET: Sprints/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint == null)
            {
                return NotFound();
            }
            return View(sprint);
        }

        // POST: Sprints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,DurationInDays,Status")] Sprint sprint)
        {
            if (id != sprint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sprint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SprintExists(sprint.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sprint);
        }

        // GET: Sprints/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sprint = await _context.Sprints
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sprint == null)
            {
                return NotFound();
            }

            return View(sprint);
        }

        // POST: Sprints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sprint = await _context.Sprints.FindAsync(id);
            if (sprint != null)
            {
                _context.Sprints.Remove(sprint);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> ManageIssues(int sprintId)
        {
            var sprint = await _context.Sprints
                .Include(s => s.Issues) // Include related issues
                .FirstOrDefaultAsync(s => s.Id == sprintId);

            if (sprint == null)
            {
                return NotFound();
            }

            var availableIssues = await _context.Issues
                .Where(i => i.SprintId == null) // Only show unassigned issues
                .ToListAsync();

            var viewModel = new SprintIssuesViewModel
            {
                SprintId = sprint.Id,
                SprintName = sprint.Name,
                AvailableIssues = availableIssues,
                SelectedIssueIds = sprint.Issues.Select(i => i.Id).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageIssues(SprintIssuesViewModel model)
        {
            var sprint = await _context.Sprints
                .Include(s => s.Issues)
                .FirstOrDefaultAsync(s => s.Id == model.SprintId);

            if (sprint == null)
            {
                return NotFound();
            }

            // Remove issues not selected
            foreach (var issue in sprint.Issues.ToList())
            {
                if (!model.SelectedIssueIds.Contains(issue.Id))
                {
                    issue.SprintId = null; // Unassign the sprint
                }
            }

            // Add selected issues
            foreach (var issueId in model.SelectedIssueIds)
            {
                var issue = await _context.Issues.FindAsync(issueId);
                if (issue != null && issue.SprintId != sprint.Id)
                {
                    issue.SprintId = sprint.Id; // Assign the sprint
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = sprint.Id });
        }


        private bool SprintExists(int id)
        {
            return _context.Sprints.Any(e => e.Id == id);
        }
    }
}

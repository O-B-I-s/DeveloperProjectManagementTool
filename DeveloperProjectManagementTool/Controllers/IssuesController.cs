using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectManagementTool.Controllers
{
    public class IssuesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HistoryLoggerService _historyLogger;


        public IssuesController(ApplicationDbContext context, HistoryLoggerService historyLogger)
        {
            _context = context;
            _historyLogger = historyLogger;

        }

        // GET: Issues
        [Authorize(Roles = ("Admin"))]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Issues.Include(i => i.Reporter)
                                                      .Include(p => p.Sprint);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Issues/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .Include(i => i.Reporter)
                .Include(p => p.Sprint)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }

            return View(issue);
        }

        // GET: Issues/Create
        public IActionResult Create(int sprintId)
        {
            var sprints = new Sprint
            {
                Id = sprintId,
            };
            //ViewData["SprintId"] = sprintId; // Pass project ID to the view

            ViewData["Type"] = new SelectList(Enum.GetValues(typeof(IssueType)).Cast<IssueType>());
            ViewData["Priority"] = new SelectList(Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>());
            ViewData["ReporterId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Issues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Summary,Description,Attachment,ReporterId,Priority,Type,Status,SprintId")] Issue issue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(issue);
                await _context.SaveChangesAsync();
                // Log history
                await _historyLogger.LogHistory(
                    "Created",
                    $"Project '{issue.Name}' was Created.",
                    projectId: issue.Id
                );

                return RedirectToAction("Details", "Sprints", new { id = issue.SprintId });
            }
            ViewData["ReporterId"] = new SelectList(_context.Users, "Id", "Id", issue.ReporterId);

            ViewData["Type"] = new SelectList(Enum.GetValues(typeof(IssueType)).Cast<IssueType>());
            ViewData["Priority"] = new SelectList(Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>());
            ViewData["SprintId"] = issue.SprintId;
            // Redirect to the Details page of the sprint that the issue belongs to
            return RedirectToAction("Details", "Sprints", new { id = issue.SprintId });
        }

        // GET: Issues/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null)
            {
                return NotFound();
            }
            ViewData["ReporterId"] = new SelectList(_context.Users, "Id", "Id", issue.ReporterId);
            ViewData["Type"] = new SelectList(Enum.GetValues(typeof(IssueType)).Cast<IssueType>());
            ViewData["Priority"] = new SelectList(Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>());
            ViewData["SprintId"] = issue.SprintId;
            return View(issue);
        }

        // POST: Issues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Summary,Description,Attachment,ReporterId,Priority,Type,Status, SprintId")] Issue issue)
        {
            if (id != issue.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(issue);
                    await _context.SaveChangesAsync();
                    await _historyLogger.LogHistory(
                          "Edited",
                          $"Issue '{issue.Name}' was updated.",
                          sprintId: issue.SprintId,
                          issueId: issue.Id
  );

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IssueExists(issue.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // Redirect to the Details page of the sprint that the issue belongs to
                return RedirectToAction("Details", "Sprints", new { id = issue.SprintId });
            }
            ViewData["ReporterId"] = new SelectList(_context.Users, "Id", "Id", issue.ReporterId);
            ViewData["Type"] = new SelectList(Enum.GetValues(typeof(IssueType)).Cast<IssueType>());
            ViewData["Priority"] = new SelectList(Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>());
            ViewData["SprintId"] = issue.SprintId;
            return View(issue);
        }

        // GET: Issues/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var issue = await _context.Issues
                .Include(i => i.Reporter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null)
            {
                return NotFound();
            }
            ViewData["Type"] = new SelectList(Enum.GetValues(typeof(IssueType)).Cast<IssueType>());
            ViewData["Priority"] = new SelectList(Enum.GetValues(typeof(PriorityLevel)).Cast<PriorityLevel>());
            ViewData["SprintId"] = issue.SprintId;
            return View(issue);
        }

        // POST: Issues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue != null)
            {
                _context.Issues.Remove(issue);
            }

            await _context.SaveChangesAsync();
            // Redirect to the Details page of the sprint that the issue belongs to
            return RedirectToAction("Details", "Sprints", new { id = issue.SprintId });
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }
    }
}

using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
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
        [Authorize(Roles = ("Admin"))]
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
                .Include(s => s.Issues)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sprint == null)
            {
                return NotFound();
            }

            return View(sprint);
        }

        // GET: Sprints/Create
        [HttpGet]
        public IActionResult Create(int projectId)
        {
            var projects = new Models.Project
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
            return RedirectToAction(nameof(Details), new { id = sprint.Id });
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
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(SprintStatus))
           .Cast<SprintStatus>()
           .Select(s => new SelectListItem
           {
               Value = s.ToString(),
               Text = s.ToString()
           }), "Value", "Text");
            return View(sprint);
        }

        // POST: Sprints/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartDate,EndDate,DurationInDays,Status, ProjectId")] Sprint sprint)
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
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!SprintExists(sprint.Id))
                    {

                        return NotFound();
                    }
                    else
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Details), new { id = sprint.Id });
            }
            ViewBag.Status = new SelectList(Enum.GetValues(typeof(SprintStatus))
                    .Cast<SprintStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = s.ToString(),
                        Text = s.ToString()
                    }), "Value", "Text");
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
                await _context.SaveChangesAsync();

            }
            return RedirectToAction(nameof(Details), new { id = sprint.Id });
        }



        [HttpGet]
        public IActionResult GetSprintsByProject(int projectId)
        {
            var sprints = _context.Sprints
                .Where(s => s.ProjectId == projectId)
                .Include(s => s.Issues)
                .ThenInclude(i => i.SubTask)
                .Select(s => new
                {
                    organizationId = s.Project.Organization.Id,
                    organizationName = s.Project.Organization.Name,
                    id = s.Id,
                    name = s.Name,
                    status = s.Status,
                    issueCount = s.Issues.Count(),
                    tasks = s.Issues.SelectMany(i => i.SubTask).Count() // Flatten issues and count tasks
                })
                .AsEnumerable() // Switch to in-memory processing
                .Select(s => new
                {
                    organizationId = s.organizationId,
                    organizationName = s.organizationName,
                    id = s.id,
                    name = s.name,
                    status = s.status,
                    issueCount = s.issueCount,
                    taskCount = s.tasks
                })
                .ToList();

            return Json(sprints);


        }


        private bool SprintExists(int id)
        {
            return _context.Sprints.Any(e => e.Id == id);
        }
    }
}

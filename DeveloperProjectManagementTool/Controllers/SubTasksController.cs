using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectManagementTool.Controllers
{
    public class SubTasksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubTasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SubTasks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SubTasks.Include(s => s.Issue);
            return View(await applicationDbContext.ToListAsync());
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
            //ViewData["IssueId"] = _context.Issues.FirstOrDefault(i => i.Id == issueId);
            return View();
        }

        // POST: SubTasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsCompleted,IssueId")] SubTask subTask)
        {
            //if (ModelState.IsValid)
            //{
            _context.Add(subTask);
            await _context.SaveChangesAsync();
            //ViewData["IssueId"] = subTask.IssueId;
            return RedirectToAction(nameof(Index));
            //}
            //ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", subTask.IssueId);
            //return View(subTask);
        }

        // GET: SubTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subTask = await _context.SubTasks.FindAsync(id);
            if (subTask == null)
            {
                return NotFound();
            }
            ViewData["IssueId"] = new SelectList(_context.Issues, "Id", "Id", subTask.IssueId);
            return View(subTask);
        }

        // POST: SubTasks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,IssueId")] SubTask subTask)
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
                return RedirectToAction(nameof(Index));
            }
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
            return RedirectToAction(nameof(Index));
        }

        private bool SubTaskExists(int id)
        {
            return _context.SubTasks.Any(e => e.Id == id);
        }
    }
}

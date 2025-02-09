using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeveloperProjectManagementTool.Controllers
{
    public class ProjectHistoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectHistoriesController(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<IActionResult> History(int projectId)
        {
            var history = await _context.ProjectHistories
                .Where(h => h.ProjectId == projectId)
                .Include(h => h.User)
                .Select(h => new ProjectHistoryViewModel
                {
                    Action = h.Action,
                    Details = h.Details,
                    UserName = h.User.UserName,
                    Timestamp = h.Timestamp
                })
                .ToListAsync();
            Console.WriteLine($"History Count: {history.Count}");
            return View(history);
        }



        private bool ProjectHistoryExists(int id)
        {
            return _context.ProjectHistories.Any(e => e.Id == id);
        }
    }
}

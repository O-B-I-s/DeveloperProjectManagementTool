using DeveloperProjectManagementTool.Data;
using DeveloperProjectManagementTool.Models;
using System.Security.Claims;

namespace DeveloperProjectManagementTool.Controllers
{
    public class HistoryLoggerService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public HistoryLoggerService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogHistory(string action, string details, int? projectId = null, int? sprintId = null, int? issueId = null, int? subTaskId = null)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            var history = new ProjectHistory
            {
                Action = action,
                Details = details,
                ProjectId = projectId,
                SprintId = sprintId,
                IssueId = issueId,
                SubTaskId = subTaskId,
                UserId = userId
            };
            Console.WriteLine($"Logging history for projectId: {projectId}, action: {action}");  // Debug log

            _context.ProjectHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}

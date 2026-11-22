using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketGo.Models;
using TicketGo.Repositories;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IEventRepository _events;
        private readonly IUserRepository _users;
        private readonly IStatsRepository _stats;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(
            IEventRepository events,
            IUserRepository users,
            IStatsRepository stats,
            UserManager<ApplicationUser> userManager)
        {
            _events = events;
            _users = users;
            _stats = stats;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = (await _users.GetAllWithRolesAsync()).ToList();

            // check which users are blocked
            foreach (var row in users)
            {
                var identityUser = await _userManager.FindByIdAsync(row.Id);
                row.IsBlocked = identityUser != null && await _userManager.IsLockedOutAsync(identityUser);
            }

            var model = new AdminDashboardViewModel
            {
                Stats = await _stats.GetAdminStatsAsync(),
                Users = users,
                Events = (await _events.GetAllAsync()).ToList()
            };

            return View(model);
        }

        // called with $.ajax, blocks or unblocks a user
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleBlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return BadRequest(new { success = false, message = "Admin accounts cannot be blocked." });

            var isBlocked = await _userManager.IsLockedOutAsync(user);

            if (isBlocked)
                await _userManager.SetLockoutEndDateAsync(user, null);
            else
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

            return Json(new { success = true, blocked = !isBlocked });
        }

        // called with $.ajax from the events table
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var deleted = await _events.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { success = false, message = "Event not found." });

            return Json(new { success = true });
        }
    }
}

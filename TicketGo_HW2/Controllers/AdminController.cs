using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = new List<UserRow>();
            foreach (var user in _userManager.Users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new UserRow { Email = user.Email ?? "", Role = string.Join(", ", roles) });
            }

            var model = new AdminDashboardViewModel
            {
                Users = users,
                Events = await _db.Events.Include(e => e.Tickets).OrderBy(e => e.Date).ToListAsync(),
                TicketsSold = await _db.Tickets.CountAsync()
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev != null)
            {
                _db.Events.Remove(ev);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Event removed.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

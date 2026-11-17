using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;

namespace TicketGo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _db.Events
                .Where(e => e.Date >= DateTime.Today)
                .OrderBy(e => e.Date)
                .Take(6)
                .ToListAsync();

            return View(events);
        }
    }
}

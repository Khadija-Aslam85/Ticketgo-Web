using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;
using TicketGo.Models;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "Organizer")]
    public class OrganizerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public OrganizerController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var events = await _db.Events
                .Include(e => e.Tickets)
                .Where(e => e.OrganizerId == userId)
                .OrderBy(e => e.Date)
                .ToListAsync();

            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new EventFormViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ev = new Event
            {
                Title = model.Title,
                Venue = model.Venue,
                Date = model.Date,
                Price = model.Price,
                TotalSeats = model.TotalSeats,
                OrganizerId = _userManager.GetUserId(User)
            };

            _db.Events.Add(ev);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.Id == id && e.OrganizerId == userId);
            if (ev == null) return NotFound();

            var model = new EventFormViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                Venue = ev.Venue,
                Date = ev.Date,
                Price = ev.Price,
                TotalSeats = ev.TotalSeats
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userManager.GetUserId(User);
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.Id == model.Id && e.OrganizerId == userId);
            if (ev == null) return NotFound();

            ev.Title = model.Title;
            ev.Venue = model.Venue;
            ev.Date = model.Date;
            ev.Price = model.Price;
            ev.TotalSeats = model.TotalSeats;
            await _db.SaveChangesAsync();

            TempData["Success"] = "Event updated.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ev = await _db.Events.FirstOrDefaultAsync(e => e.Id == id && e.OrganizerId == userId);
            if (ev != null)
            {
                _db.Events.Remove(ev);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Event deleted.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketGo.Models;
using TicketGo.Repositories;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "Organizer")]
    public class OrganizerController : Controller
    {
        private readonly IEventRepository _events;
        private readonly IStatsRepository _stats;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrganizerController(IEventRepository events, IStatsRepository stats, UserManager<ApplicationUser> userManager)
        {
            _events = events;
            _stats = stats;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            var model = new OrganizerDashboardViewModel
            {
                Stats = await _stats.GetOrganizerStatsAsync(userId),
                Events = (await _events.GetByOrganizerAsync(userId)).ToList()
            };

            return View(model);
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

            await _events.AddAsync(ev);

            TempData["Success"] = "Event created.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ev = await _events.GetByIdAsync(id);
            if (ev == null || ev.OrganizerId != userId) return NotFound();

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
            var ev = await _events.GetByIdAsync(model.Id);
            if (ev == null || ev.OrganizerId != userId) return NotFound();

            ev.Title = model.Title;
            ev.Venue = model.Venue;
            ev.Date = model.Date;
            ev.Price = model.Price;
            ev.TotalSeats = model.TotalSeats;
            await _events.UpdateAsync(ev);

            TempData["Success"] = "Event updated.";
            return RedirectToAction(nameof(Index));
        }

        // called with $.ajax from the events table
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = _userManager.GetUserId(User);
            var ev = await _events.GetByIdAsync(id);
            if (ev == null || ev.OrganizerId != userId)
                return NotFound(new { success = false, message = "Event not found." });

            await _events.DeleteAsync(id);
            return Json(new { success = true });
        }
    }
}

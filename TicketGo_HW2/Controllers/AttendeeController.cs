using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;
using TicketGo.Models;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "Attendee")]
    public class AttendeeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendeeController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var model = new AttendeeHomeViewModel
            {
                Events = await _db.Events
                    .Where(e => e.Date >= DateTime.Today)
                    .OrderBy(e => e.Date)
                    .ToListAsync(),
                Tickets = await _db.Tickets
                    .Include(t => t.Event)
                    .Where(t => t.UserId == userId)
                    .OrderByDescending(t => t.Id)
                    .ToListAsync()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            var booked = await _db.Tickets
                .Where(t => t.EventId == id)
                .Select(t => t.SeatNumber)
                .ToListAsync();

            return View(new BookViewModel { Event = ev, BookedSeats = booked.ToHashSet() });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int id, List<string> seats)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev == null) return NotFound();

            // keep only real seat numbers of this event
            var chosen = (seats ?? new List<string>())
                .Where(s => int.TryParse(s, out var n) && n >= 1 && n <= ev.TotalSeats)
                .Distinct()
                .ToList();

            if (chosen.Count == 0)
            {
                TempData["Error"] = "Please select at least one seat.";
                return RedirectToAction(nameof(Book), new { id });
            }

            var userId = _userManager.GetUserId(User)!;

            foreach (var seat in chosen)
                _db.Tickets.Add(new Ticket { EventId = id, UserId = userId, SeatNumber = seat });

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // the unique index stops two people booking the same seat
                TempData["Error"] = "One of those seats was just taken. Please choose again.";
                return RedirectToAction(nameof(Book), new { id });
            }

            TempData["Success"] = "Booking confirmed. Your tickets are in the My Tickets table.";
            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketGo.Models;
using TicketGo.Repositories;
using TicketGo.Services;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "Attendee")]
    public class AttendeeController : Controller
    {
        private readonly IEventRepository _events;
        private readonly ITicketRepository _tickets;
        private readonly IQrService _qr;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendeeController(
            IEventRepository events,
            ITicketRepository tickets,
            IQrService qr,
            UserManager<ApplicationUser> userManager)
        {
            _events = events;
            _tickets = tickets;
            _qr = qr;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User)!;

            var model = new AttendeeHomeViewModel
            {
                Events = (await _events.GetUpcomingAsync()).ToList(),
                Tickets = (await _tickets.GetByUserAsync(userId)).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Book(int id)
        {
            var ev = await _events.GetByIdAsync(id);
            if (ev == null) return NotFound();

            return View(ev);
        }

        // called with $.get, returns the seat numbers as JSON
        [HttpGet]
        public async Task<IActionResult> Seats(int id)
        {
            var ev = await _events.GetByIdAsync(id);
            if (ev == null) return NotFound();

            var booked = await _tickets.GetBookedSeatsAsync(id);
            return Json(new { total = ev.TotalSeats, booked });
        }

        // called with $.ajax, books the seats and returns JSON
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> BookSeats([FromBody] BookRequest request)
        {
            if (request == null)
                return BadRequest(new { success = false, message = "Invalid request." });

            var ev = await _events.GetByIdAsync(request.EventId);
            if (ev == null)
                return NotFound(new { success = false, message = "Event not found." });

            // keep only real seat numbers of this event
            var chosen = request.Seats
                .Where(s => int.TryParse(s, out var n) && n >= 1 && n <= ev.TotalSeats)
                .Distinct()
                .ToList();

            if (chosen.Count == 0)
                return BadRequest(new { success = false, message = "Please select at least one seat." });

            var alreadyBooked = (await _tickets.GetBookedSeatsAsync(ev.Id)).ToList();
            if (chosen.Any(s => alreadyBooked.Contains(s)))
                return Conflict(new { success = false, message = "One of those seats was just taken. Please choose again." });

            var userId = _userManager.GetUserId(User)!;
            var tickets = chosen
                .Select(seat => new Ticket { EventId = ev.Id, UserId = userId, SeatNumber = seat })
                .ToList();

            var saved = await _tickets.AddRangeAsync(tickets);
            if (!saved)
                return Conflict(new { success = false, message = "One of those seats was just taken. Please choose again." });

            return Json(new
            {
                success = true,
                message = "Booking confirmed.",
                codes = tickets.Select(t => t.TicketCode)
            });
        }

        // QR code image of one of my tickets
        [HttpGet]
        public async Task<IActionResult> Qr(string code)
        {
            var ticket = await _tickets.GetByCodeAsync(code);
            var userId = _userManager.GetUserId(User);

            if (ticket == null || ticket.UserId != userId)
                return NotFound();

            return Content(_qr.CreateSvg(ticket.TicketCode), "image/svg+xml");
        }
    }
}

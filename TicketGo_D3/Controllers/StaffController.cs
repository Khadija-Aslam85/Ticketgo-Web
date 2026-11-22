using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketGo.Repositories;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "GateStaff")]
    public class StaffController : Controller
    {
        private readonly ITicketRepository _tickets;

        public StaffController(ITicketRepository tickets)
        {
            _tickets = tickets;
        }

        public IActionResult Index()
        {
            return View();
        }

        // called with $.post, checks a ticket code and returns JSON
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(string? ticketCode)
        {
            var code = (ticketCode ?? "").Trim().ToUpper();
            var ticket = await _tickets.GetByCodeAsync(code);

            if (ticket == null)
                return Json(new { status = "notfound", message = "Ticket not found." });

            if (ticket.IsUsed)
                return Json(new { status = "used", message = "Already used. This ticket was scanned before." });

            await _tickets.MarkUsedAsync(ticket.Id);

            return Json(new
            {
                status = "valid",
                message = "Valid ticket. " + ticket.EventTitle + ", Seat " + ticket.SeatNumber + "."
            });
        }
    }
}

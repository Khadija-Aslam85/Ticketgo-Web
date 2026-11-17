using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;
using TicketGo.ViewModels;

namespace TicketGo.Controllers
{
    [Authorize(Roles = "GateStaff")]
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StaffController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new StaffCheckViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(StaffCheckViewModel model)
        {
            var code = (model.TicketCode ?? "").Trim().ToUpper();

            var ticket = await _db.Tickets
                .Include(t => t.Event)
                .FirstOrDefaultAsync(t => t.TicketCode == code);

            if (ticket == null)
            {
                model.Message = "Ticket not found.";
                model.AlertClass = "alert-warning";
            }
            else if (ticket.IsUsed)
            {
                model.Message = "Already used. This ticket was scanned before.";
                model.AlertClass = "alert-danger";
            }
            else
            {
                ticket.IsUsed = true;
                await _db.SaveChangesAsync();
                model.Message = "Valid ticket. " + ticket.Event!.Title + ", Seat " + ticket.SeatNumber + ".";
                model.AlertClass = "alert-success";
            }

            ModelState.Clear();
            model.TicketCode = "";
            return View(model);
        }
    }
}

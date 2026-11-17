using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketGo.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // sends every user to the homepage of their role
        public IActionResult Index()
        {
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Admin");
            if (User.IsInRole("Organizer")) return RedirectToAction("Index", "Organizer");
            if (User.IsInRole("GateStaff")) return RedirectToAction("Index", "Staff");
            return RedirectToAction("Index", "Attendee");
        }
    }
}

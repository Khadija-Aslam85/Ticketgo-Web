using Microsoft.AspNetCore.Mvc;
using TicketGo.Repositories;

namespace TicketGo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventRepository _events;

        public HomeController(IEventRepository events)
        {
            _events = events;
        }

        public async Task<IActionResult> Index()
        {
            var events = (await _events.GetUpcomingAsync()).Take(6).ToList();
            return View(events);
        }

        // called with $.get from the search box, returns only the event cards
        [HttpGet]
        public async Task<IActionResult> Search(string? q)
        {
            var events = await _events.GetUpcomingAsync(q);
            return PartialView("_EventCards", events);
        }
    }
}

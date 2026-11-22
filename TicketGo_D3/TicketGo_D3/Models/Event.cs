namespace TicketGo.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Venue { get; set; } = "";
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public string? OrganizerId { get; set; }
    }

    // an event plus how many seats are already booked
    public class EventSummary : Event
    {
        public int SeatsBooked { get; set; }
    }
}

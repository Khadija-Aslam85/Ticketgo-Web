namespace TicketGo.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string UserId { get; set; } = "";
        public string SeatNumber { get; set; } = "";
        public string TicketCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();
        public bool IsUsed { get; set; }
    }

    // a ticket plus the details of its event (used by joins)
    public class TicketDetail : Ticket
    {
        public string EventTitle { get; set; } = "";
        public DateTime EventDate { get; set; }
        public string EventVenue { get; set; } = "";
    }
}

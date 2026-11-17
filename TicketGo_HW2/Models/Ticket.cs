using System.ComponentModel.DataAnnotations;

namespace TicketGo.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public string UserId { get; set; } = "";

        [MaxLength(10)]
        public string SeatNumber { get; set; } = "";

        [MaxLength(20)]
        public string TicketCode { get; set; } = Guid.NewGuid().ToString("N")[..8].ToUpper();

        public bool IsUsed { get; set; }
    }
}

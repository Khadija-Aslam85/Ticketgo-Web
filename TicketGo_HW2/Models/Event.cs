using System.ComponentModel.DataAnnotations.Schema;

namespace TicketGo.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Venue { get; set; } = "";
        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int TotalSeats { get; set; }
        public string? OrganizerId { get; set; }

        public List<Ticket> Tickets { get; set; } = new();
    }
}

namespace TicketGo.Models
{
    public class UserRow
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Role { get; set; } = "";
        public bool IsBlocked { get; set; }
    }

    public class AdminStats
    {
        public int Users { get; set; }
        public int Events { get; set; }
        public int TicketsSold { get; set; }
    }

    public class OrganizerStats
    {
        public int Events { get; set; }
        public int SeatsBooked { get; set; }
        public decimal TotalSales { get; set; }
    }
}

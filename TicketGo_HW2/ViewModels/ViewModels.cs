using System.ComponentModel.DataAnnotations;
using TicketGo.Models;

namespace TicketGo.ViewModels
{
    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password), Compare("Password", ErrorMessage = "Passwords do not match.")]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; } = "";

        public string Role { get; set; } = "Attendee";
    }

    public class EventFormViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Venue { get; set; } = "";

        [Required, DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Today.AddDays(7);

        [Range(0, 1000000)]
        public decimal Price { get; set; }

        [Range(1, 500)]
        [Display(Name = "Total seats")]
        public int TotalSeats { get; set; } = 50;
    }

    public class AttendeeHomeViewModel
    {
        public List<Event> Events { get; set; } = new();
        public List<Ticket> Tickets { get; set; } = new();
    }

    public class BookViewModel
    {
        public Event Event { get; set; } = null!;
        public HashSet<string> BookedSeats { get; set; } = new();
    }

    public class UserRow
    {
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
    }

    public class AdminDashboardViewModel
    {
        public List<UserRow> Users { get; set; } = new();
        public List<Event> Events { get; set; } = new();
        public int TicketsSold { get; set; }
    }

    public class StaffCheckViewModel
    {
        [Display(Name = "Ticket code")]
        public string? TicketCode { get; set; }
        public string? Message { get; set; }
        public string AlertClass { get; set; } = "alert-info";
    }
}

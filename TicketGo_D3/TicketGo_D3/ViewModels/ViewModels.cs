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
        [Required, StringLength(60)]
        [Display(Name = "Full name")]
        public string FullName { get; set; } = "";

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
        public List<EventSummary> Events { get; set; } = new();
        public List<TicketDetail> Tickets { get; set; } = new();
    }

    public class OrganizerDashboardViewModel
    {
        public OrganizerStats Stats { get; set; } = new();
        public List<EventSummary> Events { get; set; } = new();
    }

    public class AdminDashboardViewModel
    {
        public AdminStats Stats { get; set; } = new();
        public List<UserRow> Users { get; set; } = new();
        public List<EventSummary> Events { get; set; } = new();
    }

    // body of the Ajax request that books seats
    public class BookRequest
    {
        public int EventId { get; set; }
        public List<string> Seats { get; set; } = new();
    }
}

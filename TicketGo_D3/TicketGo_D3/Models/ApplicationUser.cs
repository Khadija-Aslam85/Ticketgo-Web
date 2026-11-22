using Microsoft.AspNetCore.Identity;

namespace TicketGo.Models
{
    // customized Identity user: the default IdentityUser plus a full name
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}

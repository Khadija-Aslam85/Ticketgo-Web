using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketGo.Models;

namespace TicketGo.Data
{
    // Entity Framework is only used for the Identity tables.
    // Events and Tickets are handled with Dapper in the repositories.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}

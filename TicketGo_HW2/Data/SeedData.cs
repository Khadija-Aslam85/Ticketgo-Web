using Microsoft.AspNetCore.Identity;
using TicketGo.Models;

namespace TicketGo.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

            await db.Database.EnsureCreatedAsync();

            foreach (var role in new[] { "Admin", "Organizer", "Attendee", "GateStaff" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            await CreateUser(userManager, "admin@ticketgo.com", "Admin@123", "Admin");
            var organizer = await CreateUser(userManager, "organizer@ticketgo.com", "Organizer@123", "Organizer");
            await CreateUser(userManager, "staff@ticketgo.com", "Staff@123", "GateStaff");
            await CreateUser(userManager, "attendee@ticketgo.com", "Attendee@123", "Attendee");

            if (!db.Events.Any())
            {
                db.Events.AddRange(
                    new Event { Title = "Spring Music Night", Venue = "Main Auditorium", Date = DateTime.Today.AddDays(10), Price = 1500, TotalSeats = 40, OrganizerId = organizer.Id },
                    new Event { Title = "Tech Talk 2026", Venue = "Seminar Hall", Date = DateTime.Today.AddDays(15), Price = 500, TotalSeats = 30, OrganizerId = organizer.Id },
                    new Event { Title = "Annual Fest", Venue = "University Ground", Date = DateTime.Today.AddDays(22), Price = 800, TotalSeats = 60, OrganizerId = organizer.Id }
                );
                await db.SaveChangesAsync();
            }
        }

        private static async Task<IdentityUser> CreateUser(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser { UserName = email, Email = email };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
            }
            return user;
        }
    }
}

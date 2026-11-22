using Dapper;
using Microsoft.AspNetCore.Identity;
using TicketGo.Models;
using TicketGo.Repositories;

namespace TicketGo.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            var factory = services.GetRequiredService<IDbConnectionFactory>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var eventRepository = services.GetRequiredService<IEventRepository>();

            // 1. Identity tables (Entity Framework)
            await db.Database.EnsureCreatedAsync();

            // 2. Events and Tickets tables (Dapper)
            using (var conn = factory.Create())
            {
                await conn.ExecuteAsync(factory.IsSqlite ? Schema.Sqlite : Schema.SqlServer);
            }

            // 3. roles and demo users
            foreach (var role in new[] { "Admin", "Organizer", "Attendee", "GateStaff" })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            await CreateUser(userManager, "Admin User", "admin@ticketgo.com", "Admin@123", "Admin");
            var organizer = await CreateUser(userManager, "Ali Organizer", "organizer@ticketgo.com", "Organizer@123", "Organizer");
            await CreateUser(userManager, "Usman Staff", "staff@ticketgo.com", "Staff@123", "GateStaff");
            await CreateUser(userManager, "Sara Attendee", "attendee@ticketgo.com", "Attendee@123", "Attendee");

            // 4. sample events
            var existing = await eventRepository.GetAllAsync();
            if (!existing.Any())
            {
                await eventRepository.AddAsync(new Event { Title = "Spring Music Night", Venue = "Main Auditorium", Date = DateTime.Today.AddDays(10), Price = 1500, TotalSeats = 40, OrganizerId = organizer.Id });
                await eventRepository.AddAsync(new Event { Title = "Tech Talk 2026", Venue = "Seminar Hall", Date = DateTime.Today.AddDays(15), Price = 500, TotalSeats = 30, OrganizerId = organizer.Id });
                await eventRepository.AddAsync(new Event { Title = "Annual Fest", Venue = "University Ground", Date = DateTime.Today.AddDays(22), Price = 800, TotalSeats = 60, OrganizerId = organizer.Id });
            }
        }

        private static async Task<ApplicationUser> CreateUser(UserManager<ApplicationUser> userManager, string fullName, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser { UserName = email, Email = email, FullName = fullName };
                await userManager.CreateAsync(user, password);
                await userManager.AddToRoleAsync(user, role);
            }
            return user;
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;
using TicketGo.Models;
using TicketGo.Repositories;
using TicketGo.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------- database (SQL Server or SQLite, chosen in appsettings) ----------
var useSqlite = builder.Configuration["DatabaseProvider"] == "Sqlite";
string connection;

if (useSqlite)
{
    var dataFolder = Path.Combine(builder.Environment.ContentRootPath, "App_Data");
    Directory.CreateDirectory(dataFolder);
    var fileName = builder.Configuration["SqliteFile"] ?? "ticketgo.db";
    connection = "Data Source=" + Path.Combine(dataFolder, fileName);
}
else
{
    connection = builder.Configuration.GetConnectionString("SqlServer")!;
}

// Entity Framework is used only for the Identity tables
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
        options.UseSqlite(connection);
    else
        options.UseSqlServer(connection);
});

// Dapper connection factory, used by all the repositories
builder.Services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connection, useSqlite));

// ---------- repositories + dependency injection ----------
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStatsRepository, StatsRepository>();
builder.Services.AddSingleton<IQrService, QrService>();

// ---------- customized Identity ----------
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// puts the full name inside the login cookie
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AppClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.SlidingExpiration = true;
});

// lets the Ajax calls send the anti forgery token in a header
builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

builder.Services.AddControllersWithViews();

var app = builder.Build();

// create the tables, the roles and some sample data
using (var scope = app.Services.CreateScope())
{
    await SeedData.InitializeAsync(scope.ServiceProvider);
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

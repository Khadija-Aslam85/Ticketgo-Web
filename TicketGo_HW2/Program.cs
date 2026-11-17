using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketGo.Data;

var builder = WebApplication.CreateBuilder(args);

// pick the database from appsettings.json ("SqlServer" or "Sqlite")
var useSqlite = builder.Configuration["DatabaseProvider"] == "Sqlite";
var connection = builder.Configuration.GetConnectionString(useSqlite ? "Sqlite" : "SqlServer");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (useSqlite)
        options.UseSqlite(connection);
    else
        options.UseSqlServer(connection);
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// create the database, the roles and some sample data
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

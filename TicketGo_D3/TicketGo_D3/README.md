# TicketGo - Deliverable 3 (ASP.NET Core MVC)

Event ticketing website: attendees book seats and get QR tickets, organizers manage events,
gate staff check tickets, the admin manages users and events.

This version builds on D1 (pages) and D2 (MVC, Identity, dynamic pages) and adds the D3 topics.

## D3 checklist (where to find each item)

| # | Topic | Where |
|---|---|---|
| 1 | Ajax: `$.get`, `$.post`, `$.ajax` | `wwwroot/js/` (see table below) |
| 2 | Dapper | `Repositories/*.cs`, `Data/DbConnectionFactory.cs`, `Data/Schema.cs` |
| 3 | Repository pattern: 4 repositories, interfaces, dependency injection | `Repositories/` and the `AddScoped` lines in `Program.cs` |
| 4 | Customized Identity | `Models/ApplicationUser.cs`, `Services/AppClaimsPrincipalFactory.cs`, Identity options in `Program.cs`, `Controllers/AccountController.cs` |
| 5 | Layout sections | `Views/Shared/_Layout.cshtml` (`Styles`, `Hero`, `Scripts`) and the `@section` blocks in the views |
| 6 | Local deployment on IIS | `DEPLOYMENT_IIS.md` and `publish.bat` |
| 7 | All previous tasks | D1 pages as MVC views, D2 MVC + Identity + role homepages + dynamic data |

### 1. Ajax
| Call | File | What it does |
|---|---|---|
| `$.get` | `search.js` | live search of events (Home and Attendee page) |
| `$.get` | `book.js` | loads the seat map as JSON |
| `$.ajax` (POST, JSON) | `book.js` | books the selected seats without reloading the page |
| `$.ajax` (POST) | `site.js` | deletes an event (Organizer and Admin tables) |
| `$.ajax` (POST) | `site.js` | blocks or unblocks a user (Admin page) |
| `$.post` | `staff.js` | gate staff ticket check |

All POST calls send the anti forgery token.

### 2 and 3. Dapper and repositories
Entity Framework is used only for the Identity tables (users, roles).
Events and Tickets are plain SQL with Dapper, inside four repositories:

| Interface | Class | Job |
|---|---|---|
| `IEventRepository` | `EventRepository` | events: list, search, add, update, delete |
| `ITicketRepository` | `TicketRepository` | tickets: book seats (transaction), find by code, mark used |
| `IUserRepository` | `UserRepository` | users joined with their roles (admin page) |
| `IStatsRepository` | `StatsRepository` | numbers for the admin and organizer dashboards |

The controllers only know the interfaces. The classes are given to them by dependency injection (`Program.cs`).

### 4. Identity customization
- `ApplicationUser` adds a `FullName` to the default user
- full name is asked on sign up and shown in the navbar (added to the login cookie by `AppClaimsPrincipalFactory`)
- own password rules, unique email, lockout after 5 wrong passwords
- admin can block or unblock users (uses Identity lockout)
- own Login, Register, Logout and Access Denied pages with roles: Admin, Organizer, Attendee, GateStaff

## Run
1. Install the .NET SDK (8 or newer).
2. Open a terminal in this folder and run `dotnet run`
3. Open http://localhost:5080

The database, roles and sample data are created on the first run.

**Database:** when you run it from Visual Studio or `dotnet run`, it uses SQL Server LocalDB (installed with Visual Studio).
If LocalDB is not on your PC, change `"DatabaseProvider": "SqlServer"` to `"Sqlite"` in `appsettings.json`.
When published to IIS it uses SQLite automatically (`appsettings.Production.json`), so nothing else has to be installed.
The database name is new (`TicketGoD3Db`), so it does not clash with the D2 database.

## Demo accounts
| Role | Email | Password |
|---|---|---|
| Admin | admin@ticketgo.com | Admin@123 |
| Organizer | organizer@ticketgo.com | Organizer@123 |
| Gate Staff | staff@ticketgo.com | Staff@123 |
| Attendee | attendee@ticketgo.com | Attendee@123 |

## Try it (demo flow)
1. Login as the attendee, open an event, pick seats, confirm. Open My Tickets and click View QR.
2. Copy the ticket code, login as gate staff, verify it. Verify it again to see "Already used".
3. Login as the organizer, create an event, delete it from the table.
4. Login as the admin, block the attendee, then try to login as the attendee.

## Notes
- jQuery and Bootstrap are inside `wwwroot/lib`, so the site works without internet.
- If you change the Identity user class later, delete the database first (EnsureCreated does not update tables).

# TicketGo - Deliverable 2 (ASP.NET Core MVC)

Event ticketing website. Models, Views and Controllers, with ASP.NET Core Identity login.

## Run
1. Install the .NET SDK (8 or newer) and open this folder in Visual Studio, or a terminal.
2. `dotnet run`
3. Open http://localhost:5080

The database, roles and sample data are created automatically on the first run.

## Database
Default is SQL Server LocalDB (installed with Visual Studio).
If LocalDB is not on your PC, open `appsettings.json` and change
`"DatabaseProvider": "SqlServer"` to `"Sqlite"`. Nothing else to install.

If you change the models later, delete the database first (EnsureCreated does not update tables).

## Demo accounts
| Role | Email | Password |
|---|---|---|
| Admin | admin@ticketgo.com | Admin@123 |
| Organizer | organizer@ticketgo.com | Organizer@123 |
| Gate Staff | staff@ticketgo.com | Staff@123 |
| Attendee | attendee@ticketgo.com | Attendee@123 |

New users can also sign up as Attendee or Organizer.

## What works
- Identity register, login and logout, with roles
- After login each role goes to its own homepage
- Home page shows events from the database
- Attendee: browse events, pick seats (no double booking), My Tickets table with ticket codes
- Organizer: create, edit and delete own events, see seats booked and sales
- Admin: see users, events and tickets sold, remove events
- Gate Staff: check a ticket code, valid tickets are marked as used

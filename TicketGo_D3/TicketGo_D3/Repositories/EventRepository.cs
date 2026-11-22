using Dapper;
using TicketGo.Data;
using TicketGo.Models;

namespace TicketGo.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly IDbConnectionFactory _factory;

        public EventRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        private const string SummarySelect = @"
SELECT e.Id, e.Title, e.Venue, e.[Date], e.Price, e.TotalSeats, e.OrganizerId,
       (SELECT COUNT(*) FROM Tickets t WHERE t.EventId = e.Id) AS SeatsBooked
FROM Events e ";

        public async Task<IEnumerable<EventSummary>> GetUpcomingAsync(string? search = null)
        {
            var sql = SummarySelect + "WHERE e.[Date] >= @Today";

            if (!string.IsNullOrWhiteSpace(search))
                sql += " AND (e.Title LIKE @Pattern OR e.Venue LIKE @Pattern)";

            sql += " ORDER BY e.[Date]";

            var pattern = "%" + (search ?? "").Trim() + "%";

            using var conn = _factory.Create();
            return await conn.QueryAsync<EventSummary>(sql, new { Today = DateTime.Today, Pattern = pattern });
        }

        public async Task<IEnumerable<EventSummary>> GetAllAsync()
        {
            using var conn = _factory.Create();
            return await conn.QueryAsync<EventSummary>(SummarySelect + "ORDER BY e.[Date]");
        }

        public async Task<IEnumerable<EventSummary>> GetByOrganizerAsync(string organizerId)
        {
            using var conn = _factory.Create();
            return await conn.QueryAsync<EventSummary>(
                SummarySelect + "WHERE e.OrganizerId = @OrganizerId ORDER BY e.[Date]",
                new { OrganizerId = organizerId });
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            const string sql = "SELECT Id, Title, Venue, [Date], Price, TotalSeats, OrganizerId FROM Events WHERE Id = @Id";

            using var conn = _factory.Create();
            return await conn.QueryFirstOrDefaultAsync<Event>(sql, new { Id = id });
        }

        public async Task<int> AddAsync(Event ev)
        {
            var sql = @"INSERT INTO Events (Title, Venue, [Date], Price, TotalSeats, OrganizerId)
                        VALUES (@Title, @Venue, @Date, @Price, @TotalSeats, @OrganizerId); " + _factory.LastIdSql;

            using var conn = _factory.Create();
            return await conn.ExecuteScalarAsync<int>(sql, ev);
        }

        public async Task<bool> UpdateAsync(Event ev)
        {
            const string sql = @"UPDATE Events
                                 SET Title = @Title, Venue = @Venue, [Date] = @Date, Price = @Price, TotalSeats = @TotalSeats
                                 WHERE Id = @Id";

            using var conn = _factory.Create();
            var rows = await conn.ExecuteAsync(sql, ev);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();

            // tickets first, then the event
            await conn.ExecuteAsync("DELETE FROM Tickets WHERE EventId = @Id", new { Id = id }, tx);
            var rows = await conn.ExecuteAsync("DELETE FROM Events WHERE Id = @Id", new { Id = id }, tx);

            tx.Commit();
            return rows > 0;
        }
    }
}

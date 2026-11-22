using System.Data.Common;
using Dapper;
using TicketGo.Data;
using TicketGo.Models;

namespace TicketGo.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly IDbConnectionFactory _factory;

        public TicketRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        private const string DetailSelect = @"
SELECT t.Id, t.EventId, t.UserId, t.SeatNumber, t.TicketCode, t.IsUsed,
       e.Title AS EventTitle, e.[Date] AS EventDate, e.Venue AS EventVenue
FROM Tickets t
INNER JOIN Events e ON e.Id = t.EventId ";

        public async Task<IEnumerable<TicketDetail>> GetByUserAsync(string userId)
        {
            using var conn = _factory.Create();
            return await conn.QueryAsync<TicketDetail>(
                DetailSelect + "WHERE t.UserId = @UserId ORDER BY t.Id DESC",
                new { UserId = userId });
        }

        public async Task<IEnumerable<string>> GetBookedSeatsAsync(int eventId)
        {
            using var conn = _factory.Create();
            return await conn.QueryAsync<string>(
                "SELECT SeatNumber FROM Tickets WHERE EventId = @EventId",
                new { EventId = eventId });
        }

        public async Task<TicketDetail?> GetByCodeAsync(string code)
        {
            using var conn = _factory.Create();
            return await conn.QueryFirstOrDefaultAsync<TicketDetail>(
                DetailSelect + "WHERE t.TicketCode = @Code",
                new { Code = code });
        }

        // saves all the tickets or none of them
        public async Task<bool> AddRangeAsync(IEnumerable<Ticket> tickets)
        {
            const string sql = @"INSERT INTO Tickets (EventId, UserId, SeatNumber, TicketCode, IsUsed)
                                 VALUES (@EventId, @UserId, @SeatNumber, @TicketCode, @IsUsed)";

            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                foreach (var ticket in tickets)
                    await conn.ExecuteAsync(sql, ticket, tx);

                tx.Commit();
                return true;
            }
            catch (DbException)
            {
                // the unique index on (EventId, SeatNumber) stops a seat being booked twice
                tx.Rollback();
                return false;
            }
        }

        public async Task<bool> MarkUsedAsync(int ticketId)
        {
            using var conn = _factory.Create();
            var rows = await conn.ExecuteAsync(
                "UPDATE Tickets SET IsUsed = 1 WHERE Id = @Id AND IsUsed = 0",
                new { Id = ticketId });
            return rows > 0;
        }
    }
}

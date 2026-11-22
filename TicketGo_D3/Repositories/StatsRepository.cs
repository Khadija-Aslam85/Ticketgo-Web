using Dapper;
using TicketGo.Data;
using TicketGo.Models;

namespace TicketGo.Repositories
{
    public class StatsRepository : IStatsRepository
    {
        private readonly IDbConnectionFactory _factory;

        public StatsRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<AdminStats> GetAdminStatsAsync()
        {
            const string sql = @"
SELECT (SELECT COUNT(*) FROM AspNetUsers) AS Users,
       (SELECT COUNT(*) FROM Events) AS Events,
       (SELECT COUNT(*) FROM Tickets) AS TicketsSold";

            using var conn = _factory.Create();
            return await conn.QuerySingleAsync<AdminStats>(sql);
        }

        public async Task<OrganizerStats> GetOrganizerStatsAsync(string organizerId)
        {
            using var conn = _factory.Create();

            var events = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Events WHERE OrganizerId = @Id",
                new { Id = organizerId });

            // one price per sold ticket, added up here
            var prices = (await conn.QueryAsync<decimal>(
                @"SELECT e.Price FROM Tickets t
                  INNER JOIN Events e ON e.Id = t.EventId
                  WHERE e.OrganizerId = @Id",
                new { Id = organizerId })).ToList();

            return new OrganizerStats
            {
                Events = events,
                SeatsBooked = prices.Count,
                TotalSales = prices.Sum()
            };
        }
    }
}

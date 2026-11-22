using TicketGo.Models;

namespace TicketGo.Repositories
{
    public interface IStatsRepository
    {
        Task<AdminStats> GetAdminStatsAsync();
        Task<OrganizerStats> GetOrganizerStatsAsync(string organizerId);
    }
}

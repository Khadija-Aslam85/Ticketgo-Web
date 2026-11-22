using TicketGo.Models;

namespace TicketGo.Repositories
{
    public interface IEventRepository
    {
        Task<IEnumerable<EventSummary>> GetUpcomingAsync(string? search = null);
        Task<IEnumerable<EventSummary>> GetAllAsync();
        Task<IEnumerable<EventSummary>> GetByOrganizerAsync(string organizerId);
        Task<Event?> GetByIdAsync(int id);
        Task<int> AddAsync(Event ev);
        Task<bool> UpdateAsync(Event ev);
        Task<bool> DeleteAsync(int id);
    }
}

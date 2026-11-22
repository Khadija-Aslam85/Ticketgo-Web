using TicketGo.Models;

namespace TicketGo.Repositories
{
    public interface ITicketRepository
    {
        Task<IEnumerable<TicketDetail>> GetByUserAsync(string userId);
        Task<IEnumerable<string>> GetBookedSeatsAsync(int eventId);
        Task<TicketDetail?> GetByCodeAsync(string code);
        Task<bool> AddRangeAsync(IEnumerable<Ticket> tickets);
        Task<bool> MarkUsedAsync(int ticketId);
    }
}

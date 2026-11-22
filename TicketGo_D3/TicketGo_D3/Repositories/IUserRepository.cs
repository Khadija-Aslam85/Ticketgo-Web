using TicketGo.Models;

namespace TicketGo.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserRow>> GetAllWithRolesAsync();
    }
}

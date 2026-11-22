using Dapper;
using TicketGo.Data;
using TicketGo.Models;

namespace TicketGo.Repositories
{
    // reads the Identity tables with Dapper (joins users with their roles)
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _factory;

        public UserRepository(IDbConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task<IEnumerable<UserRow>> GetAllWithRolesAsync()
        {
            const string sql = @"
SELECT u.Id,
       COALESCE(u.Email, '') AS Email,
       COALESCE(u.FullName, '') AS FullName,
       COALESCE(r.Name, '') AS [Role]
FROM AspNetUsers u
LEFT JOIN AspNetUserRoles ur ON ur.UserId = u.Id
LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
ORDER BY u.Email";

            using var conn = _factory.Create();
            return await conn.QueryAsync<UserRow>(sql);
        }
    }
}

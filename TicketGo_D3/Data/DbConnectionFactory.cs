using System.Data;
using Microsoft.Data.Sqlite;
using Microsoft.Data.SqlClient;

namespace TicketGo.Data
{
    public interface IDbConnectionFactory
    {
        bool IsSqlite { get; }

        // sql that returns the id of the row that was just inserted
        string LastIdSql { get; }

        IDbConnection Create();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public bool IsSqlite { get; }

        public string LastIdSql => IsSqlite
            ? "SELECT last_insert_rowid();"
            : "SELECT CAST(SCOPE_IDENTITY() AS INT);";

        public DbConnectionFactory(string connectionString, bool isSqlite)
        {
            _connectionString = connectionString;
            IsSqlite = isSqlite;
        }

        public IDbConnection Create()
        {
            if (IsSqlite)
                return new SqliteConnection(_connectionString);

            return new SqlConnection(_connectionString);
        }
    }
}

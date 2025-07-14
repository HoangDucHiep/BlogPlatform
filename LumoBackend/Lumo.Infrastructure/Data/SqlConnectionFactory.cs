using System.Data;
using Lumo.Application.Abstractions.Data;
using Npgsql;

namespace Lumo.Infrastructure.Data;

// <summary>
// SqlConnectionFactory is responsible for creating and managing SQL database connections.
// It implements the ISqlConnectionFactory interface, which defines a method for creating database connections.
// </summary>
/// <remarks>
/// This class uses Npgsql to connect to a PostgreSQL database.
/// It is initialized with a connection string that specifies the database to connect to.
/// The CreateConnection method opens a new connection to the database and returns it as an IDbConnection.
/// </remarks>
public class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;
    
    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    /// <returns>An open IDbConnection to the PostgreSQL database.</returns>
    /// remarks>
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        return connection;
    }
}

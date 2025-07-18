using System.Data;

namespace Lumo.Application.Abstractions.Data;

/// <summary>
/// Interface for creating SQL database connections.
/// </summary>
public interface ISqlConnectionFactory
{
    /// <summary>
    /// Creates a new database connection.
    /// </summary>
    IDbConnection CreateConnection();
}

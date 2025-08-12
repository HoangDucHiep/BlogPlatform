using System.Data;
using Dapper;
using Lumo.Application.Builders;

namespace Lumo.Application.Extensions;

/// <summary>
/// Extension methods for IDbConnection to work with SqlBuilder.
/// </summary>
public static class SqlBuilderExtensions
{
    /// <summary>
    /// Executes a query using SqlBuilder and returns the results with pagination.
    /// </summary>
    /// <typeparam name="T">The type to map the results to</typeparam>
    /// <param name="connection">The database connection</param>
    /// <param name="sqlBuilder">The configured SqlBuilder instance</param>
    /// <returns>A tuple containing the results and total count</returns>
    public static async Task<(List<T> Results, int TotalCount)> QueryWithPaginationAsync<T>(
        this IDbConnection connection,
        SqlBuilder sqlBuilder)
    {
        var mainQuery = sqlBuilder.Build();
        var countQuery = sqlBuilder.BuildCountQuery();

        // Combine both queries into one string separated by semicolon
        var combinedSql = $"{mainQuery};{countQuery}";

        using var gridReader = await connection.QueryMultipleAsync(combinedSql, sqlBuilder.Parameters);
        var results = (await gridReader.ReadAsync<T>()).ToList();
        var totalCount = await gridReader.ReadFirstAsync<int>();

        return (results, totalCount);
    }

    /// <summary>
    /// Executes a query using SqlBuilder and returns the results.
    /// </summary>
    /// <typeparam name="T">The type to map the results to</typeparam>
    /// <param name="connection">The database connection</param>
    /// <param name="sqlBuilder">The configured SqlBuilder instance</param>
    /// <returns>The query results</returns>
    public static async Task<List<T>> QueryAsync<T>(
        this IDbConnection connection,
        SqlBuilder sqlBuilder)
    {
        var sql = sqlBuilder.Build();
        var results = await connection.QueryAsync<T>(sql, sqlBuilder.Parameters);
        return results.ToList();
    }

    /// <summary>
    /// Executes a query using SqlBuilder and returns the first result or default.
    /// </summary>
    /// <typeparam name="T">The type to map the result to</typeparam>
    /// <param name="connection">The database connection</param>
    /// <param name="sqlBuilder">The configured SqlBuilder instance</param>
    /// <returns>The first result or default value</returns>
    public static async Task<T?> QueryFirstOrDefaultAsync<T>(
        this IDbConnection connection,
        SqlBuilder sqlBuilder)
    {
        var sql = sqlBuilder.Build();
        return await connection.QueryFirstOrDefaultAsync<T>(sql, sqlBuilder.Parameters);
    }

    /// <summary>
    /// Executes a query using SqlBuilder and returns a single result.
    /// </summary>
    /// <typeparam name="T">The type to map the result to</typeparam>
    /// <param name="connection">The database connection</param>
    /// <param name="sqlBuilder">The configured SqlBuilder instance</param>
    /// <returns>A single result</returns>
    public static async Task<T> QuerySingleAsync<T>(
        this IDbConnection connection,
        SqlBuilder sqlBuilder)
    {
        var sql = sqlBuilder.Build();
        return await connection.QuerySingleAsync<T>(sql, sqlBuilder.Parameters);
    }

    /// <summary>
    /// Executes a count query using SqlBuilder.
    /// </summary>
    /// <param name="connection">The database connection</param>
    /// <param name="sqlBuilder">The configured SqlBuilder instance</param>
    /// <param name="countField">The field to count (default: "*")</param>
    /// <returns>The count result</returns>
    public static async Task<int> QueryCountAsync(
        this IDbConnection connection,
        SqlBuilder sqlBuilder,
        string countField = "*")
    {
        var sql = sqlBuilder.BuildCountQuery(countField);
        return await connection.QuerySingleAsync<int>(sql, sqlBuilder.Parameters);
    }
}

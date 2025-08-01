using System.Data;
using System.Text.RegularExpressions;
using Dapper;
using Dapper.SimpleSqlBuilder.FluentBuilder;

namespace Lumo.Application.Extensions;

public static class PaginationHelper
{
    private static readonly Regex SingleTableRegex = new(
        @"\bFROM\s+(?:\[?(\w+)\]?\.)?(?:\[?(\w+)\]?)",
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    private static readonly Regex JoinRegex = new(
        @"\b(?:INNER\s+JOIN|LEFT\s+JOIN|RIGHT\s+JOIN|FULL\s+JOIN|CROSS\s+JOIN|JOIN)\b",
        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);

    public static ISelectFromBuilder ApplyPagination(this ISelectFromBuilder builder, int page, int pageSize)
    {
        if (page < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be greater than or equal to 1.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than or equal to 1.");
        }

        var offset = (page - 1) * pageSize;

        var limitBuilder = (ILimitBuilder)builder;
        limitBuilder.Limit(pageSize)
            .Offset((page - 1) * pageSize);

        return builder;
    }

    /// <summary>
    /// Validates that SQL query contains only a single table (no JOINs)
    /// </summary>
    /// <param name="sqlQuery">SQL query to validate</param>
    /// <returns>True if single table, false if multiple tables or JOINs found</returns>
    public static bool IsSingleTableQuery(string sqlQuery)
    {
        if (string.IsNullOrWhiteSpace(sqlQuery))
        {
            return false;
        }

        // Normalize the query
        var normalizedSql = Regex.Replace(sqlQuery.Trim(), @"\s+", " ");

        // Check for JOINs - if found, it's not a single table query
        if (JoinRegex.IsMatch(normalizedSql))
        {
            return false;
        }

        // Check for multiple FROM clauses (subqueries)
        var fromMatches = Regex.Matches(normalizedSql, @"\bFROM\b", RegexOptions.IgnoreCase);
        if (fromMatches.Count > 1)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Extracts table name from single-table query and validates it
    /// </summary>
    /// <param name="sqlQuery">SQL query</param>
    /// <returns>Table name if valid single table query, null otherwise</returns>
    public static string? FindSingleTableName(string sqlQuery)
    {
        if (!IsSingleTableQuery(sqlQuery))
        {
            return null;
        }

        var normalizedSql = Regex.Replace(sqlQuery.Trim(), @"\s+", " ");
        var match = SingleTableRegex.Match(normalizedSql);

        if (match.Success)
        {
            // Return table name (without schema and alias)
            return match.Groups[2].Success ? match.Groups[2].Value : match.Groups[1].Value;
        }

        return null;
    }

    /// <summary>
    /// Gets paginated result by executing both data and count queries in a single connection call
    /// </summary>
    /// <typeparam name="T">Type to map query results to</typeparam>
    /// <param name="sqlQuery">SQL query for getting the data</param>
    /// <param name="expectedTableName">Expected table name for validation</param>
    /// <param name="connection">Database connection</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Query results and total count</returns>
    public static async Task<(IEnumerable<T> Data, int TotalCount)> GetPaginatedResult<T>(
    string sqlQuery,
    string expectedTableName,
    IDbConnection connection,
    object? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(sqlQuery))
        {
            throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));
        }

        if (string.IsNullOrWhiteSpace(expectedTableName))
        {
            throw new ArgumentException("Expected table name cannot be null or empty.", nameof(expectedTableName));
        }

        // Validate single table query
        var foundTableName = FindSingleTableName(sqlQuery);
        if (foundTableName == null)
        {
            throw new InvalidOperationException("Query contains multiple tables or JOINs. Single table queries only.");
        }

        if (!string.Equals(foundTableName, expectedTableName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Table name mismatch. Expected: '{expectedTableName}', Found: '{foundTableName}'");
        }

        // Chuẩn hóa sqlQuery
        var normalizedSqlQuery = Regex.Replace(sqlQuery.Trim(), @"[\r\n\s]+", " ");

        // Chạy truy vấn phân trang
        var data = await connection.QueryAsync<T>(normalizedSqlQuery, parameters);

        // Chạy truy vấn đếm
        var countSql = $"SELECT COUNT(*) FROM {expectedTableName}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        return (data, totalCount);
    }

    /// <summary>
    /// Gets paginated result with auto-detected table name using single connection call
    /// </summary>
    /// <typeparam name="T">Type to map query results to</typeparam>
    /// <param name="sqlQuery">SQL query for getting the data</param>
    /// <param name="connection">Database connection</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Query results and total count</returns>
    public static async Task<(IEnumerable<T> Data, int TotalCount)> GetPaginatedResultAuto<T>(
        string sqlQuery,
        IDbConnection connection,
        object? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(sqlQuery))
        {
            throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));
        }

        // Auto-detect table name
        var tableName = FindSingleTableName(sqlQuery);

        if (tableName == null)
        {
            throw new InvalidOperationException(
                "Cannot auto-detect table name. Query must be a simple single-table SELECT without JOINs.");
        }

        // Combine both queries into a single multi-query call
        var countSql = $"SELECT COUNT(*) FROM {tableName}";
        var combinedSql = $"{sqlQuery}; {countSql}";

        using var multiQuery = await connection.QueryMultipleAsync(combinedSql, parameters);

        var data = await multiQuery.ReadAsync<T>();
        var totalCount = await multiQuery.ReadSingleAsync<int>();

        return (data, totalCount);
    }

    /// <summary>
    /// Gets paginated result with custom count query using single connection call
    /// </summary>
    /// <typeparam name="T">Type to map query results to</typeparam>
    /// <param name="sqlQuery">SQL query for getting the data</param>
    /// <param name="countQuery">Custom count query</param>
    /// <param name="connection">Database connection</param>
    /// <param name="parameters">Query parameters</param>
    /// <returns>Query results and total count</returns>
    public static async Task<(IEnumerable<T> Data, int TotalCount)> GetPaginatedResultWithCustomCount<T>(
        string sqlQuery,
        string countQuery,
        IDbConnection connection,
        object? parameters = null)
    {
        if (string.IsNullOrWhiteSpace(sqlQuery))
        {
            throw new ArgumentException("SQL query cannot be null or empty.", nameof(sqlQuery));
        }

        if (string.IsNullOrWhiteSpace(countQuery))
        {
            throw new ArgumentException("Count query cannot be null or empty.", nameof(countQuery));
        }

        // Combine both queries into a single multi-query call
        var combinedSql = $"{sqlQuery}; {countQuery}";

        using var multiQuery = await connection.QueryMultipleAsync(combinedSql, parameters);

        var data = await multiQuery.ReadAsync<T>();
        var totalCount = await multiQuery.ReadSingleAsync<int>();

        return (data, totalCount);
    }
}

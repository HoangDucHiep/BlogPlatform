using System.Data;
using Dapper;

namespace Lumo.Application.Extensions;
public static class DbConnectionExtensions
{
    public async static Task<(List<T> lstRes, int count)> GetQueryWithPagination<T>(
        this IDbConnection connection,
        string getSqlQuery,
        object? parameters,
        string countTablesName,
        string whereFilter
        )
    {

        string countQuery = $"SELECT COUNT(*) FROM {countTablesName} {whereFilter}";

        // Combine both queries into one string separated by semicolon
        string combinedSql = $"{getSqlQuery};{countQuery}";

        var gridReader = await connection.QueryMultipleAsync(combinedSql, parameters);
        var stories = (await gridReader.ReadAsync<T>()).ToList();
        var totalCount = await gridReader.ReadFirstAsync<int>();

        return (stories, totalCount);
    }
}

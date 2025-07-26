using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.SimpleSqlBuilder.FluentBuilder;
using Lumo.Application.Abstractions.Data;
using MediatR;

namespace Lumo.Application.Extensions;



public static class PaginationHelper
{
    public static ISelectFromBuilder Pagination(this ISelectFromBuilder builder, int page, int pageSize)
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

    public static async Task<int> GetTotalCount(string tableName, IDbConnection connection)
    {
        if (string.IsNullOrWhiteSpace(tableName))
        {
            throw new ArgumentException("Table name cannot be null or empty.", nameof(tableName));
        }
        var sql = $"""SELECT COUNT(*) FROM {tableName}""";
        
        var totalCount = await connection.ExecuteScalarAsync<int>(sql);
        return totalCount;
    }
}

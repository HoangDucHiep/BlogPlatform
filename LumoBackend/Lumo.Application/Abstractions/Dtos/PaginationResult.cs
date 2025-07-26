using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Application.Abstractions.Dtos;
public sealed record PaginationResult<T> : ICollectionResponse<T>
{
    public List<T> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PaginationResult<T> CreateAsync(
        List<T> source, 
        int page, 
        int pageSize)
    {
        int totalCount = source.Count;

        return new PaginationResult<T>
        {
            Items = source,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }
}

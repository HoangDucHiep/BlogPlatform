using System.Text.Json.Serialization;

namespace Lumo.Application.Abstractions.Dtos;
public sealed record PaginationResult<T> : ICollectionResponse<T>
{
    public List<T> Items { get; init; }
    [JsonPropertyName("metadata")]
    public PaginationMetadata MetaData { get; init; }

    public static PaginationResult<T> Create(
        List<T> source,
        int page,
        int pageSize,
        int totalCount)
    {

        return new PaginationResult<T>
        {
            Items = source,
            MetaData = new PaginationMetadata
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        };
    }
}

public sealed record PaginationMetadata
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

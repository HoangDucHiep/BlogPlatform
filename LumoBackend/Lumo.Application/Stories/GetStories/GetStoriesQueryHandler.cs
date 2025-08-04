using Dapper;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Helpers;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Stories;

namespace Lumo.Application.Stories.GetStories;
public class GetStoriesQueryHandler : IQueryHandler<GetStoriesQuery, PaginationResult<StoryDto>>
{
    private readonly IStoryRepository _storyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;


    public GetStoriesQueryHandler(IStoryRepository storyRepository, IUnitOfWork unitOfWork, ISqlConnectionFactory sqlConnectionFactory)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
    }

    public async Task<Result<PaginationResult<StoryDto>>> Handle(GetStoriesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        string sortQuery = SortingHelper.ParseSortQuery(request.Sort, new List<string>
        {
            "title",
            "created_at_utc",
            "content",
            "author_id",
        });

        // Filter conditions
        var whereConditions = new List<string>();

        if (request.Status.HasValue)
        {
            whereConditions.Add("s.status = @Status");
        }

        if (request.AuthorId.HasValue)
        {
            whereConditions.Add("s.author_id = @AuthorId");
        }

        if (request.PublicationId.HasValue)
        {
            whereConditions.Add("s.publication_id = @PublicationId");
        }

        if (request.IsPaywalled.HasValue)
        {
            whereConditions.Add("s.is_paywalled = @IsPaywalled");
        }

        // Combine where conditions into a single string
        string whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : string.Empty;

        // search clause
        if (!string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            whereClause += (whereClause.Length > 0 ? " AND " : "WHERE ") +
                           $"(s.title ILIKE '%' || @SearchQuery || '%' OR s.content ILIKE '%' || @SearchQuery || '%')";
        }

        string mainSql = $"""
            SELECT
                s.id AS Id,
                s.title AS Title,
                s.slug AS Slug,
                s.content AS Content,
                s.author_id AS AuthorId,
                s.publication_id AS PublicationId,
                s.status AS Status,
                s.published_at_utc AS PublishedAtUtc,
                s.is_paywalled AS IsPaywalled,
                s.read_time_calculated AS ReadTimeCalculated,
                s.created_at_utc AS CreatedAtUtc,
                s.last_updated_at_utc AS LastUpdatedAtUtc
            FROM stories AS s
            {whereClause}
            ORDER BY {sortQuery ?? "s.created_at_utc DESC"}
            LIMIT @PageSize OFFSET @Offset
            """;

        string countSql = $"""
            SELECT COUNT(*) FROM stories AS s
            {whereClause}
            """;

        var parameters = new
        {
            request.PageSize,
            Offset = (request.Page - 1) * request.PageSize,
            request.Status,
            request.AuthorId,
            request.PublicationId,
            request.IsPaywalled,
            SearchQuery = request.SearchQuery?.Trim() ?? string.Empty
        };

        // Combine both queries into one string separated by semicolon
        string combinedSql = $"{mainSql};{countSql}";

        var gridReader = await connection.QueryMultipleAsync(combinedSql, parameters);
        var stories = (await gridReader.ReadAsync<StoryDto>()).ToList();
        var totalCount = await gridReader.ReadFirstAsync<int>();

        var paginationResult = PaginationResult<StoryDto>.CreateAsync(
            stories,
            request.Page,
            request.PageSize,
            totalCount
        );

        return Result.Success(paginationResult);
    }
}

using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Builders;
using Lumo.Application.Extensions;
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

        // Build SQL query using SqlBuilder
        var sqlBuilder = SqlBuilder.Create()
            .Select(
                "s.id AS Id",
                "s.title AS Title",
                "s.slug AS Slug",
                "s.content AS Content",
                "s.author_id AS AuthorId",
                "s.publication_id AS PublicationId",
                "s.status AS Status",
                "s.published_at_utc AS PublishedAtUtc",
                "s.is_paywalled AS IsPaywalled",
                "s.read_time_calculated AS ReadTimeCalculated",
                "s.created_at_utc AS CreatedAtUtc",
                "s.last_updated_at_utc AS LastUpdatedAtUtc"
            )
            .From("stories AS s")
            .WhereIf(request.Status.HasValue, "s.status = @Status", "Status", request.Status!)
            .WhereIf(request.AuthorId.HasValue, "s.author_id = @AuthorId", "AuthorId", request.AuthorId!)
            .WhereIf(request.PublicationId.HasValue, "s.publication_id = @PublicationId", "PublicationId", request.PublicationId!)
            .WhereIf(request.IsPaywalled.HasValue, "s.is_paywalled = @IsPaywalled", "IsPaywalled", request.IsPaywalled!)
            .Search(request.SearchQuery ?? string.Empty, ["s.title", "s.content"])
            .OrderBy(request.Sort ?? "s.created_at_utc DESC")
            .Paginate(request.Page, request.PageSize);

        // Execute query with pagination
        var (stories, totalCount) = await connection.QueryWithPaginationAsync<StoryDto>(sqlBuilder);

        var paginationResult = PaginationResult<StoryDto>.CreateAsync(
            stories,
            request.Page,
            request.PageSize,
            totalCount
        );

        return Result.Success(paginationResult);
    }
}

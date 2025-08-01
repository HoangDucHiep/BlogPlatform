using Dapper.SimpleSqlBuilder;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
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

        var builder = SimpleBuilder.CreateFluent()
            .Select($"""
            s.id AS Id,
            s.title AS Title,
            s.content AS Content,
            s.author_id AS AuthorId,
            s.publication_id AS PublicationId,
            s.status AS Status,
            s.published_at_utc AS PublishedAtUtc,
            s.is_paywalled AS IsPaywalled,
            s.read_time_calculated AS ReadTimeCalculated,
            s.created_at_utc AS CreatedAtUtc,
            s.last_updated_at_utc AS LastUpdatedAtUtc
            """)
            .From($"stories s");

        builder.ApplyPagination(request.Page, request.PageSize);
        builder.ApplySorting(request.Sort);

        // Tạo class để map kết quả với TotalCount
        var (stories, totalCount) = await PaginationHelper.GetPaginatedResult<StoryDto>(
        builder.Sql,
        "stories",
        connection,
        builder.Parameters);

        if (!stories.Any())
        {
            return Result.Failure<PaginationResult<StoryDto>>(StoryError.NotFound);
        }

        var paginationResult = PaginationResult<StoryDto>.CreateAsync(
            stories.ToList(),
            request.Page,
            request.PageSize,
            totalCount);

        return Result.Success(paginationResult);
    }
}

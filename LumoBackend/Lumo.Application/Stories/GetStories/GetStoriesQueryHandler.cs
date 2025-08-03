using Dapper;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
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

        string mainSql = $"""
            SELECT
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
            FROM stories AS s
            ORDER BY {request.Sort ?? "s.created_at_utc DESC"}
            LIMIT @PageSize OFFSET @Offset
            """;

        string countSql = """
            SELECT COUNT(*) FROM stories AS s
            """;

        var parameters = new
        {
            request.PageSize,
            Offset = (request.Page - 1) * request.PageSize,
        };

        // Combine both queries into one string separated by semicolon
        string combinedSql = $"{mainSql};{countSql}";

        var gridReader = await connection.QueryMultipleAsync(combinedSql, parameters);
        var stories = (await gridReader.ReadAsync<StoryDto>()).ToList();
        var totalCount = await gridReader.ReadFirstAsync<int>();

        if (!stories.Any())
        {
            return Result.Failure<PaginationResult<StoryDto>>(StoryError.NotFound);
        }

        var paginationResult = PaginationResult<StoryDto>.CreateAsync(
            stories,
            totalCount,
            request.Page,
            request.PageSize
        );

        return Result.Success(paginationResult);
    }
}

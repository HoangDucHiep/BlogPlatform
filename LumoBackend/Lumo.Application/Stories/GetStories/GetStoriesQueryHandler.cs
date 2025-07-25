using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Stories;

namespace Lumo.Application.Stories.GetStories;
public class GetStoriesQueryHandler : IQueryHandler<GetStoriesQuery, IEnumerable<StoryDto>>
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

    public async Task<Result<IEnumerable<StoryDto>>> Handle(GetStoriesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        string sql = """
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
            FROM stories s
            """;

        string whereClause = string.Empty;

        if (request.AuthorId.HasValue)
        {
            whereClause = " s.author_id = @AuthorId";
        }

        if (!string.IsNullOrEmpty(whereClause))
        {
            sql += " WHERE" + whereClause;
        }

        var stories = await connection.QueryAsync<StoryDto>(
            sql,
            new { request.AuthorId }
            );

        if (stories is null || !stories.Any())
        {
            return Result.Failure<IEnumerable<StoryDto>>(new Error("Story.NotFound", "Story not found"));
        }

        return Result.Success(stories);
    }
}

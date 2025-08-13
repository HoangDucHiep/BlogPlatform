using Dapper;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Builders;
using Lumo.Application.Extensions;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Stories;

namespace Lumo.Application.Stories.GetSaveChangeHistoryByStory;
public class GetSaveChangeHistoryByStoryQueryHandler : IQueryHandler<GetSaveChangeHistoryByStoryQuery, PaginationResult<SaveChangeVersionDto>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserContext _userContext;

    public GetSaveChangeHistoryByStoryQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IUserContext userContext)
    {
        _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
    }

    public async Task<Result<PaginationResult<SaveChangeVersionDto>>> Handle(GetSaveChangeHistoryByStoryQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        // Validate if story exists and belongs to the user
        var authorId = await _userContext.UserId();

        // Simple approach: Get the author_id if story exists, null if not
        var storyAuthorId = await ValidateStoryOwnerShip(request.StoryId);

        // Story doesn't exist
        if (storyAuthorId == null)
        {
            return Result.Failure<PaginationResult<SaveChangeVersionDto>>(StoryError.NotFound);
        }

        // Story exists but user is not the owner
        if (storyAuthorId != authorId)
        {
            return Result.Failure<PaginationResult<SaveChangeVersionDto>>(StoryError.Forbidden);
        }

        // Build SQL query using SqlBuilder
        var sqlBuilder = SqlBuilder.Create()
            .Select(
                "scv.id AS Id",
                "scv.story_id AS StoryId",
                "scv.title AS Title",
                "scv.content AS Content",
                "scv.description AS Description",
                "scv.created_at_utc AS CreatedAtUtc"
            )
            .From("save_change_versions AS scv")
            .Where("scv.story_id = @StoryId", "StoryId", request.StoryId)
            .OrderBy("scv.created_at_utc DESC")
            .Paginate(request.Page, request.PageSize);

        // Execute the query and map results to SaveChangeVersionDto
        var (histories, totalCount) = await connection.QueryWithPaginationAsync<SaveChangeVersionDto>(sqlBuilder);

        // Return the result wrapped in a Result object
        var paginationResult = PaginationResult<SaveChangeVersionDto>.Create(
            histories,
            request.Page,
            request.PageSize,
            totalCount
        );

        return Result.Success(paginationResult);
    }


    /// <summary>
    /// Validates if the story exists and belongs to the user.
    /// </summary>
    /// <param name="storyId"></param>
    /// <param name="authorId"></param>
    /// <returns>null if story doesn't exist or doesn't belong to the user, the author_id if it does</returns>
    private async Task<Guid?> ValidateStoryOwnerShip(Guid storyId)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();
        // Check if the story exists and belongs to the user
        var storyAuthorId = await connection.QueryFirstOrDefaultAsync<Guid?>(
            "SELECT author_id FROM stories WHERE id = @StoryId",
            new { StoryId = storyId }
        );

        return storyAuthorId;
    }
}

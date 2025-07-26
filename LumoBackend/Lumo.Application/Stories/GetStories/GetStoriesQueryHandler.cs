using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.SimpleSqlBuilder;
using Dapper.SimpleSqlBuilder.FluentBuilder;
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

        builder.Pagination(request.Page, request.PageSize);

        // Tạo class để map kết quả với TotalCount
        var result = await connection.QueryAsync<StoryDto>(builder.Sql, builder.Parameters);
        var totalCount = await PaginationHelper.GetTotalCount("stories", connection);

        if (result == null || !result.Any())
        {
            return Result.Failure<PaginationResult<StoryDto>>(StoryError.NotFound);
        }

        var stories = result.Select(r => new StoryDto
        {
            Id = r.Id,
            Title = r.Title,
            Content = r.Content,
            AuthorId = r.AuthorId,
            PublicationId = r.PublicationId,
            Status = r.Status,
            PublishedAtUtc = r.PublishedAtUtc,
            IsPaywalled = r.IsPaywalled,
            ReadTimeCalculated = r.ReadTimeCalculated,
            CreatedAtUtc = r.CreatedAtUtc,
            LastUpdatedAtUtc = r.LastUpdatedAtUtc
        }).ToList();


        var paginationResult = new PaginationResult<StoryDto>
        {
            Items = stories,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };

        return Result.Success(paginationResult);
    }

}

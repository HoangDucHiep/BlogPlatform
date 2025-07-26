using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Dapper.SimpleSqlBuilder.DependencyInjection;
using Dapper.SimpleSqlBuilder.FluentBuilder;
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
    private readonly ISimpleBuilder _simpleSqlBuilder;


    public GetStoriesQueryHandler(IStoryRepository storyRepository, IUnitOfWork unitOfWork, ISqlConnectionFactory sqlConnectionFactory, ISimpleBuilder simpleSqlBuilder)
    {
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
        _simpleSqlBuilder = simpleSqlBuilder ?? throw new ArgumentNullException(nameof(simpleSqlBuilder));
    }

    public async Task<Result<PaginationResult<StoryDto>>> Handle(GetStoriesQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        var builder = _simpleSqlBuilder.CreateFluent()
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

        // Execute query
        var stories = await connection.QueryAsync<StoryDto>(
            builder.Sql,
            builder.Parameters);

        if (stories == null || !stories.Any())
        {
            return Result.Failure<PaginationResult<StoryDto>>(StoryError.NotFound);
        }

        // Assuming PaginationResult is a class that needs to be instantiated
        var paginationResult = PaginationResult<StoryDto>.CreateAsync(
            stories.ToList(),
            1,
            100);

        return Result.Success(paginationResult);
    }
}

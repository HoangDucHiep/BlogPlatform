using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Stories;
using Microsoft.AspNetCore.Mvc;

namespace Lumo.Application.Stories.GetStories;
public record GetStoriesQuery : IQuery<PaginationResult<StoryDto>>, IPageableRequest
{
    public Guid? AuthorId { get; set; }
    public StoryStatus? Status { get; set; }
    public Guid? PublicationId { get; set; }

    [FromQuery(Name = "page")]
    public int Page { get; init; } = 2;

    [FromQuery(Name = "pageSize")]
    public int PageSize { get; init; } = 4;
}

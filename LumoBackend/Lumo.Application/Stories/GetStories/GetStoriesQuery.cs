using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Dtos;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Stories;

namespace Lumo.Application.Stories.GetStories;
public record GetStoriesQuery : IQuery<PaginationResult<StoryDto>>
{
    public Guid? AuthorId { get; set; }
    public StoryStatus? Status { get; set; }
    public Guid? PublicationId { get; set; }
}

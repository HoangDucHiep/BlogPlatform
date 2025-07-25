using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;

namespace Lumo.Application.Stories.GetStories;
public record GetStoriesQuery : IQuery<IEnumerable<StoryDto>>
{
    public Guid? AuthorId { get; set; }
}

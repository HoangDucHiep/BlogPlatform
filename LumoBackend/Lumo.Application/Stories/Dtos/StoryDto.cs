using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Users.Dtos;
using Lumo.Domain.Stories;
using Lumo.Domain.Users;
using Lumo.Domain.Utils;

namespace Lumo.Application.Stories.Dtos;
public record StoryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid AuthorId { get; set; }
    public Guid? PublicationId { get; set; }
    public StoryStatus Status { get; set; }
    public DateTimeOffset? PublishedAtUtc { get; set; }
    public bool IsPaywalled { get; set; }
    public bool ReadTimeCalculated { get; set; }

    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset LastUpdatedAtUtc { get; init; }


}

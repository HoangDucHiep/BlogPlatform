using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Users;
using Lumo.Domain.Utils;

namespace Lumo.Domain.Stories;
public sealed class Story : Entity
{
    public string Title { get; private set; }
    public string Content { get; private set; }
    public Guid AuthorId { get; private set; }
    public Guid? PublicationId { get; private set; }
    public StoryStatus Status { get; private set; }
    public DateTimeOffset? PublishedAtUtc { get; private set; }
    public bool IsPaywalled { get; private set; }
    public bool ReadTimeCalculated { get; private set; }

    // Navigation properties
    public User Author { get; private set; }
    //public Publication Publication { get; private set; }
    public ICollection<SaveChangeVersion> SaveChangeVersions { get; private set; } = new List<SaveChangeVersion>();

    public static Story CreateDraft(Guid AuthorId, string Title = "", string Content = "")
    {
        return new Story
        {
            Id = IdGenerator.GenerateId(),
            AuthorId = AuthorId,
            Title = Title,
            Content = Content,
            Status = StoryStatus.Draft,
        };
    }
}

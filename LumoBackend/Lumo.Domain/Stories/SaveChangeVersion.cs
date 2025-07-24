using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Utils;

namespace Lumo.Domain.Stories;
public sealed class SaveChangeVersion : Entity
{
    public Guid StoryId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string Description { get; private set; }

    // Navigation properties
    public Story Story { get; private set; }

    public static SaveChangeVersion Create(Guid storyId, string title, string content, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));
        }
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content cannot be null or empty.", nameof(content));
        }
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));
        }
        return new SaveChangeVersion
        {
            Id = IdGenerator.GenerateId(),
            StoryId = storyId,
            Title = title,
            Content = content,
            Description = description
        };
    }
}

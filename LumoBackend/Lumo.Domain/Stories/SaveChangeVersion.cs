using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Stories;
public sealed class SaveChangeVersion : Entity
{
    public Guid StoryId { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public string Description { get; private set; }

    // Navigation properties
    public Story Story { get; private set; }
}

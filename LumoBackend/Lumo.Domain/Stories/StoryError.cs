using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Abstractions;

namespace Lumo.Domain.Stories;
public static class StoryError
{
    public static readonly Error NotFound = new(
        "Story.Found",
        "The story with the specified identifier was not found");
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Domain.Stories.ValueObjects;
public sealed record ReadTime
{
    public int Minutes { get; init; }
    public ReadTime(int minutes)
    {
        if (minutes < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minutes), "Read time cannot be negative.");
        }
        Minutes = minutes;
    }
}

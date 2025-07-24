using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Domain.Stories;
public interface ISaveChangeVersionRepository
{
    Task<SaveChangeVersion?> GetByIdAsync(Guid versionId, CancellationToken cancellationToken = default);

    Task<SaveChangeVersion> AddAsync(SaveChangeVersion story, CancellationToken cancellationToken = default);

    Task<SaveChangeVersion> GetByStoryIdAsync(Guid storyId, CancellationToken cancellationToken = default);
}

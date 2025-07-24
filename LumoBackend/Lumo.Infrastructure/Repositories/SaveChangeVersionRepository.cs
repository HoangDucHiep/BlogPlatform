using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Stories;
using Microsoft.EntityFrameworkCore;

namespace Lumo.Infrastructure.Repositories;
public sealed class SaveChangeVersionRepository : Repository<SaveChangeVersion>, ISaveChangeVersionRepository
{
    public SaveChangeVersionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {

    }

    public async Task<SaveChangeVersion> GetByStoryIdAsync(Guid storyId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<SaveChangeVersion>()
            .FirstOrDefaultAsync(scv => scv.StoryId == storyId, cancellationToken);
    }
}

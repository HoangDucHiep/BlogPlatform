using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Stories;

namespace Lumo.Infrastructure.Repositories;
public sealed class StoryRepository : Repository<Story>, IStoryRepository
{
    public StoryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

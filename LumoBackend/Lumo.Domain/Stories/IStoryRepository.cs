using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Users;

namespace Lumo.Domain.Stories;
public interface IStoryRepository
{
    Task<Story?> GetByIdAsync(Guid storyId, CancellationToken cancellationToken = default);

    Task<Story> AddAsync(Story story, CancellationToken cancellationToken = default);

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Domain.Stories;
public interface IStoryRepository
{
    Task<Story> AddNewDraft(Story story);

      
}

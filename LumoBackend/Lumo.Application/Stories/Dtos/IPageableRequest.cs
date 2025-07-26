using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Application.Stories.Dtos;
public interface IPageableRequest
{
    int Page { get; init; }
    int PageSize { get; init; }
}

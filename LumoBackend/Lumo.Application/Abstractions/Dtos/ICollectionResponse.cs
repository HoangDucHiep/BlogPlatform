using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumo.Application.Abstractions.Dtos;
public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}

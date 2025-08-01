namespace Lumo.Application.Abstractions.Dtos;
public interface ISortableRequest
{
    // sort=name desc,description,frequency.type&pageSize=1
    string Sort { get; init; }
}

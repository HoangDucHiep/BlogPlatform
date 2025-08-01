namespace Lumo.Application.Abstractions.Dtos;
public interface IPageableRequest
{
    int Page { get; init; }
    int PageSize { get; init; }
}

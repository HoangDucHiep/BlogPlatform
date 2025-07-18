using Lumo.Domain.Abstractions;
using MediatR;

namespace Lumo.Application.Abstractions.Messaging;

/// <summary>
/// Represents a query that returns a <see cref="Result"/> indicating success or failure.
/// </summary>
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

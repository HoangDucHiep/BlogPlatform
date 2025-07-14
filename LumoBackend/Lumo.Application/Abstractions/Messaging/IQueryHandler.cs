using Lumo.Domain.Abstractions;
using MediatR;

namespace Lumo.Application.Abstractions.Messaging;

/// <summary>
/// Represents a query handler that processes queries and returns a <see cref="Result{TResponse}"/>.
/// </summary>
public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
    where TResponse : Entity
{

}

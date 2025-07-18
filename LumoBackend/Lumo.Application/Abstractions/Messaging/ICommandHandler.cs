using Lumo.Domain.Abstractions;
using MediatR;

namespace Lumo.Application.Abstractions.Messaging;

/// <summary>
/// Represents a command handler that processes commands and returns a <see cref="Result"/>.
/// </summary>
public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result>
    where TCommand : ICommand
{
}


/// <summary>
/// Represents a command handler that processes commands and returns a <see cref="Result{TResponse}"/>.
/// </summary>
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, Result<TResponse>>
    where TCommand : ICommand<TResponse>
{
}

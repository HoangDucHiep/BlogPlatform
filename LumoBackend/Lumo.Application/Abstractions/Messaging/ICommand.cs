using Lumo.Domain.Abstractions;
using MediatR;

namespace Lumo.Application.Abstractions.Messaging;

/// <summary>
/// Represents a command that returns a <see cref="Result"/> indicating success or failure.
/// </summary>
public interface ICommand : IRequest<Result>, IBaseCommand
{
}

/// <summary>
/// Represents a command that returns a <see cref="Result{TResponse}"/> containing a value of type <typeparamref name="TResponse"/> on success.
/// </summary>
/// <typeparam name="TResponse">The type of the response value returned on successful command execution.</typeparam>
public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}

/// <summary>
/// Base interface for all command types in the application.
/// </summary>
public interface IBaseCommand
{ 
}

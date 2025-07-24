using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Users;

namespace Lumo.Application.Users.RegisterUser;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository, 
        IUnitOfWork unitOfWork, 
        IAuthenticationService authenticationService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(
            new Name(request.UserName),
            new EmailAddress(request.Email)
        );

        string identity = await _authenticationService.RegisterUserAsync(
            user,
            request.Password,
            cancellationToken);
        
        user.SetIdentityId(identity);

        await _userRepository.AddAsync(user, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}

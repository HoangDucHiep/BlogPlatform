using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Users.RegisterUser;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Stories;
using Lumo.Domain.Users;
using MediatR;

namespace Lumo.Application.Stories.CreateNewDraft;
public sealed class CreateNewDraftCommandHandler : ICommandHandler<CreateNewDraftCommand, DraftDto>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStoryRepository _storyRepository;
    private readonly ISaveChangeVersionRepository _saveChangeVersionRepository;

    public CreateNewDraftCommandHandler(
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IStoryRepository storyRepository,
        ISaveChangeVersionRepository saveChangeVersionRepository)
    {
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _saveChangeVersionRepository = saveChangeVersionRepository ?? throw new ArgumentNullException(nameof(saveChangeVersionRepository));
    }



    public async Task<Result<DraftDto>> Handle(CreateNewDraftCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var loggedInUserId = await _userContext.UserId();

            var newDraft = Story.CreateDraft(loggedInUserId, request.Title, request.Content);

            var savedVersion = SaveChangeVersion.Create(newDraft.Id, request.Title, request.Content, "Draft created");

            var savedDraft = await _storyRepository.AddAsync(newDraft, cancellationToken);
            await _saveChangeVersionRepository.AddAsync(savedVersion, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new DraftDto
            {
                Id = savedDraft.Id,
                Title = savedDraft.Title,
                Content = savedDraft.Content,
                CreatedAtUtc = savedDraft.CreatedAtUtc,
                LastUpdatedAtUtc = savedDraft.LastUpdatedAtUtc
            };
        }
        catch (ApplicationException)
        {
            return Result.Failure<DraftDto>(UserErrors.NotFound);
        }
    }
}

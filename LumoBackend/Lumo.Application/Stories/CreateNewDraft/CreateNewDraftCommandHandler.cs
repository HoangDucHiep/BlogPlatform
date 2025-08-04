using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Clock;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Application.Stories.Dtos;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Stories;
using Lumo.Domain.Users;
using Slugify;

namespace Lumo.Application.Stories.CreateNewDraft;
public sealed class CreateNewDraftCommandHandler : ICommandHandler<CreateNewDraftCommand, StoryDto>
{
    private readonly IUserContext _userContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStoryRepository _storyRepository;
    private readonly ISaveChangeVersionRepository _saveChangeVersionRepository;
    private readonly SlugHelper _slugHelper = new SlugHelper();
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateNewDraftCommandHandler(
        IUserContext userContext,
        IUnitOfWork unitOfWork,
        IStoryRepository storyRepository,
        ISaveChangeVersionRepository saveChangeVersionRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _storyRepository = storyRepository ?? throw new ArgumentNullException(nameof(storyRepository));
        _saveChangeVersionRepository = saveChangeVersionRepository ?? throw new ArgumentNullException(nameof(saveChangeVersionRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }



    public async Task<Result<StoryDto>> Handle(CreateNewDraftCommand request, CancellationToken cancellationToken)
    {
        try
        {

            if (request.Title == null || request.Content == null)
            {
                return Result.Failure<StoryDto>(StoryError.InvalidInput);
            }

            var loggedInUserId = await _userContext.UserId();

            var newDraft = Story.CreateDraft(loggedInUserId, request.Title, request.Content, _slugHelper.GenerateSlug($"{request.Title} {_dateTimeProvider.UtcNowOffset.ToUnixTimeSeconds()}"));

            var savedVersion = SaveChangeVersion.Create(newDraft.Id, request.Title, request.Content, "Draft created");

            var savedDraft = await _storyRepository.AddAsync(newDraft, cancellationToken);
            await _saveChangeVersionRepository.AddAsync(savedVersion, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new StoryDto
            {
                Id = savedDraft.Id,
                Title = savedDraft.Title,
                Slug = savedDraft.Slug,
                Content = savedDraft.Content,
                AuthorId = savedDraft.AuthorId,
                CreatedAtUtc = savedDraft.CreatedAtUtc,
                LastUpdatedAtUtc = savedDraft.LastUpdatedAtUtc
            };
        }
        catch (ApplicationException)
        {
            return Result.Failure<StoryDto>(UserErrors.NotFound);
        }
    }
}

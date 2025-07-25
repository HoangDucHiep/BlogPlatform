using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Domain.Users;

namespace Lumo.Application.Users.Dtos;
public class UserDto
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; }
    public string EmailAddress { get; private set; }
    public string Bio { get; private set; } = string.Empty;
    public string ProfilePictureUrl { get; private set; } = string.Empty;
    public string CoverPictureUrl { get; private set; } = string.Empty;
    public string? SocialLinks { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset LastUpdatedAtUtc { get; init; }
}

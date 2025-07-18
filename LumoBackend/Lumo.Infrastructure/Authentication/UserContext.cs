using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Lumo.Infrastructure.Authentication;
public sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public Guid UserId() => _httpContextAccessor
        .HttpContext
        ?.User.GetUserId()
        ?? throw new ApplicationException("User identifier is unavailable");

    public string IdentityId() => _httpContextAccessor
        .HttpContext
        ?.User.GetIdentityId()
        ?? throw new ApplicationException("User identity is unavailable");
}

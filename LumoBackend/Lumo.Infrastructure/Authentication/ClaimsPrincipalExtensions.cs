using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Data;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Lumo.Infrastructure.Authentication;
public static class ClaimsPrincipalExtensions
{
    public static string GetIdentityId(this ClaimsPrincipal? principal)
    {
        // Try multiple claim types based on your token structure
        var identityId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);

        return identityId ?? throw new ApplicationException("User identity is unavailable");
    }
}

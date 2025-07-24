using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Data;
using Microsoft.AspNetCore.Http;

namespace Lumo.Infrastructure.Authentication;
public sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public UserContext(IHttpContextAccessor httpContextAccessor, ISqlConnectionFactory sqlConnectionFactory)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
    }

    /// <summary>
    /// Retrieves the user ID associated with the current HTTP context's identity.
    /// This method queries the database to find the user ID based on the identity ID
    /// extracted from the HTTP context.
    /// Throws an exception if the user is not found or if the identity ID is unavailable.  
    /// </summary>
    /// <returns>
    /// A <see cref="Guid"/> representing the user ID.
    /// </returns>
    /// <exception cref="ApplicationException">
    /// Thrown when the user is not found for the given identity ID or if the identity ID is unavailable.
    /// </exception>
    public async Task<Guid> UserId()
    {
        string identityId = IdentityId();

        var connection = _sqlConnectionFactory.CreateConnection();

        string query = """
            SELECT
            	id AS Id
            FROM users
            WHERE identity_id = @IdentityId
            """;

        var userId = await connection.QuerySingleAsync<Guid>(
            query,
            new { IdentityId = identityId });

        if (userId == Guid.Empty)
        {
            throw new ApplicationException("User not found for the given identity ID.");
        }

        return userId;

    }

    public string IdentityId() => _httpContextAccessor
        .HttpContext
        ?.User.GetIdentityId()
        ?? throw new ApplicationException("User identity is unavailable");
}

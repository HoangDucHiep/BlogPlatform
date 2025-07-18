using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Messaging;
using Lumo.Domain.Abstractions;

namespace Lumo.Application.Users.GetLoggedInUser;
public sealed class GetLoggedInUserQueryHandler
    : IQueryHandler<GetLoggedInUserQuery, UserResponse>
{

    private readonly IUserContext _userContext;
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetLoggedInUserQueryHandler(
        IUserContext userContext,
        ISqlConnectionFactory sqlConnectionFactory)
    {
        _userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        _sqlConnectionFactory = sqlConnectionFactory ?? throw new ArgumentNullException(nameof(sqlConnectionFactory));
    }

    public async Task<Result<UserResponse>> Handle(
        GetLoggedInUserQuery request,
        CancellationToken cancellationToken)
    {
        // Log the identity ID for debugging
        string identityId = _userContext.IdentityId();
        Console.WriteLine($"Getting user info for identity ID: {identityId}");

        using var connection = _sqlConnectionFactory.CreateConnection();

        const string sql = """
            SELECT
            	id AS Id,
            	user_name AS UserName,
            	email_address AS Email
            FROM users
            WHERE identity_id = @IdentityId
            """;

        //Console.WriteLine($"Executing SQL query with identity_id = {identityId}");
        
        var user = await connection.QuerySingleAsync<UserResponse>(
            sql,
            new
            {
                IdentityId = identityId
            });

        if (user is null)
        {
            return Result.Failure<UserResponse>(
                new Error(
                    "UserNotFound",
                    "The user associated with the provided identity ID was not found."));
        }

        return Result.Success(user);
    }
}

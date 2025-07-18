using System.Net.Http.Json;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Domain.Users;
using Lumo.Infrastructure.Authentication.Models;

namespace Lumo.Infrastructure.Authentication;

public sealed class AuthenticationService : IAuthenticationService
{
    private const string PASSWORD_CREDENTIAL_TYPE = "password";
    private readonly HttpClient _httpClient;

    public AuthenticationService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> RegisterUserAsync(
        User user,
        string password,
        CancellationToken cancellationToken = default)
    {
        var userRepresentationModel = UserRepresentationModel.FromUser(user);

        userRepresentationModel.Credentials =
        [
            new()
            {
                Value = password,
                Temporary = false,
                Type = PASSWORD_CREDENTIAL_TYPE,
            }
        ];

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync(
            "users", 
            userRepresentationModel, 
            cancellationToken);

        return ExtractIdentityIdFromLocationHeader(response);
    }

    private static string ExtractIdentityIdFromLocationHeader(
        HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";


        string? locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;

        if (string.IsNullOrEmpty(locationHeader))
        {
            throw new InvalidOperationException("Location header is missing.");
        }

        int userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        string userIdentityId = locationHeader.Substring(
            userSegmentValueIndex + usersSegmentName.Length);

        return userIdentityId;
    }
}

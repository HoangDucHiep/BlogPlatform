using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Domain.Abstractions;
using Lumo.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;

namespace Lumo.Infrastructure.Authentication;
public sealed class JwtService : IJwtService
{

    private static readonly Error AuthenticationFailed = new(
        "Keycloak authentication failed.",
        "Failed to authenticate with Keycloak. Please check your credentials and try again.");

    private readonly KeycloakOptions _keycloakOptions;
    private readonly HttpClient _httpClient;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }

    public async Task<Result<string>> GetAccessTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var authRequestParams = new KeyValuePair<string, string>[]
            {
                new("client_id", _keycloakOptions.AuthClientId),
                new("client_secret", _keycloakOptions.AuthClientSecret),
                new("scope", "openid profile email offline_access"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password)
            };

            using var authRequestBody = new FormUrlEncodedContent(authRequestParams);

            var response = await _httpClient.PostAsync(
                "",
                authRequestBody,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var authorizationResponse = await response.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken);

            if (authorizationResponse is null || string.IsNullOrEmpty(authorizationResponse.AccessToken))
            {
                return Result.Failure<string>(AuthenticationFailed);
            }

            return Result.Success(authorizationResponse.AccessToken);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<string>(AuthenticationFailed);
        }
    }
}

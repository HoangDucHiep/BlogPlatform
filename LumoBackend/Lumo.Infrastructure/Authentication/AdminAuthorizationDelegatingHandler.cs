using System.Net.Http.Headers;
using System.Net.Http.Json;
using Lumo.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Lumo.Infrastructure.Authentication;

/// <summary>
/// Handles authentication with Keycloak admin credentials for outgoing HTTP requests.
/// This delegating handler automatically adds JWT bearer tokens to outgoing requests
/// by retrieving an access token from Keycloak using client credentials flow.
/// </summary>
public sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly KeycloakOptions _keycloakOptions;
    private readonly HttpClient _tokenHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminAuthorizationDelegatingHandler"/> class.
    /// </summary>
    /// <param name="keycloakOptions">Configuration options for connecting to Keycloak.</param>
    /// <param name="httpClientFactory">Factory for creating HTTP clients.</param>
    /// <exception cref="ArgumentNullException">Thrown when parameters are null.</exception>
    public AdminAuthorizationDelegatingHandler(
        IOptions<KeycloakOptions> keycloakOptions,
        IHttpClientFactory httpClientFactory)
    {
        _keycloakOptions = keycloakOptions.Value ?? throw new ArgumentNullException(nameof(keycloakOptions));
        _tokenHttpClient = httpClientFactory.CreateClient();
    }

    /// <summary>
    /// Sends an HTTP request to the inner handler with an added authorization header.
    /// </summary>
    /// <param name="request">The HTTP request message to send to the server.</param>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>The HTTP response message received from the server.</returns>
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Obtain a token from Keycloak using client credentials
        AuthorizationToken authorizationToken = await GetAuthorizationToken(cancellationToken);

        // Add the token to the request authorization header
        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            authorizationToken.AccessToken);

        // Send the request to the inner handler
        HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

        // Throw if the response is not successful
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            string errorContent = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);

            throw new HttpRequestException(
                $"Keycloak Admin API request failed. " +
                $"Status: {httpResponseMessage.StatusCode}, " +
                $"Request URL: {request.RequestUri}, " +
                $"Request Method: {request.Method}, " +
                $"Error Content: {errorContent}");
        }


        return httpResponseMessage;
    }

    /// <summary>
    /// Retrieves an authorization token from Keycloak using client credentials flow.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
    /// <returns>An <see cref="AuthorizationToken"/> containing the access token.</returns>
    /// <exception cref="InvalidOperationException">Thrown when unable to retrieve a valid token.</exception>
    private async Task<AuthorizationToken> GetAuthorizationToken(CancellationToken cancellationToken)
    {
        // Create parameters for the token request
        var authorizationRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", _keycloakOptions.AdminClientId),
            new("client_secret", _keycloakOptions.AdminClientSecret),
            new("scope", "openid email"),
            new("grant_type", "client_credentials")
        };

        // Create the form content for the request and properly dispose it
        using var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

        // Use the separate HttpClient to avoid infinite recursion
        HttpResponseMessage authorizationResponse = await _tokenHttpClient.PostAsync(
            _keycloakOptions.TokenUrl,
            authorizationRequestContent,
            cancellationToken);

        // Ensure we got a successful response
        authorizationResponse.EnsureSuccessStatusCode();

        // Parse the response and return the token
        return await authorizationResponse.Content
            .ReadFromJsonAsync<AuthorizationToken>(cancellationToken)
               ?? throw new InvalidOperationException("Failed to retrieve authorization token.");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _tokenHttpClient?.Dispose();
        }
        base.Dispose(disposing);
    }
}

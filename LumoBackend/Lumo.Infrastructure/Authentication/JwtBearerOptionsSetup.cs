using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Lumo.Infrastructure.Authentication;

public class JwtBearerOptionsSetup : IConfigureOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions _authenticationOptions;
    private readonly ILogger<JwtBearerOptionsSetup> _logger;

    public JwtBearerOptionsSetup(
        IOptions<AuthenticationOptions> authenticationOptions,
        ILogger<JwtBearerOptionsSetup> logger)
    {
        _authenticationOptions = authenticationOptions.Value;
        _logger = logger;
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5404:Do not disable token validation checks", Justification = "Keycloak does not include audience claim in Resource Owner Password Grant")]
    public void Configure(JwtBearerOptions options)
    {
        _logger.LogInformation("JwtBearerOptionsSetup.Configure() is being called!");
        _logger.LogInformation("ValidIssuer: {ValidIssuer}", _authenticationOptions.ValidIssuer);
        _logger.LogInformation("Audience: {Audience}", _authenticationOptions.Audience);
        _logger.LogInformation("MetadataUrl: {MetadataUrl}", _authenticationOptions.MetadataUrl);

        options.Authority = _authenticationOptions.ValidIssuer;
        options.RequireHttpsMetadata = _authenticationOptions.RequireHttpsMetadata;
        options.MetadataAddress = _authenticationOptions.MetadataUrl;

        // Configure token validation parameters
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _authenticationOptions.ValidIssuer,
            ValidateAudience = false, // Disable audience validation since Keycloak doesn't include 'aud' claim
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            NameClaimType = "preferred_username",
            RoleClaimType = "realm_access.roles"
        };

        // Add debugging events
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                _logger.LogError("JWT Authentication failed: {Message}", context.Exception.Message);
                _logger.LogError("Exception details: {Exception}", context.Exception);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                _logger.LogInformation("JWT Token validated successfully");
                var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogInformation("Claims: {Claims}", string.Join(", ", claims ?? Array.Empty<string>()));
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                _logger.LogWarning("JWT Authentication challenge: {Error}, {ErrorDescription}", context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                _logger.LogInformation("JWT token received for validation");
                var token = context.Token;
                _logger.LogInformation("Token: {Token}", token?.Length > 10 ? string.Concat(token.AsSpan(0, 10), "...") : token);
                return Task.CompletedTask;
            }
        };

        _logger.LogInformation("JwtBearerOptionsSetup.Configure() completed successfully!");
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}

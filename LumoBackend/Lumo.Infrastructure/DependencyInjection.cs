using System.Collections;
using Lumo.Application.Abstractions.Authentication;
using Lumo.Application.Abstractions.Clock;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Email;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Users;
using Lumo.Infrastructure.Authentication;
using Lumo.Infrastructure.Clock;
using Lumo.Infrastructure.Data;
using Lumo.Infrastructure.Email;
using Lumo.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Lumo.Infrastructure;

/// <summary>
/// Provides extension methods for registering infrastructure services in the application's service collection.
/// This class handles the registration of database, authentication, and other infrastructure services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all infrastructure services required by the application.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The application configuration containing connection strings and other settings.</param>
    /// <returns>The modified service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register utility services
        services.AddTransient<IDateTimeProvider, DatetimeProvider>(); // Provides consistent date/time operations
        services.AddTransient<IEmailService, EmailService>(); // Handles email sending functionality

        // Register database and persistence services
        AddPersistence(services, configuration);

        // Register authentication and authorization services
        AddAuthentication(services, configuration);

        return services;
    }

    /// <summary>
    /// Configures database connection and registers repository services.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration containing connection strings.</param>
    /// <exception cref="InvalidOperationException">Thrown when the 'Default' connection string is missing.</exception>
    /// <remarks>
    /// This method sets up:
    /// - PostgreSQL database connection with snake case naming convention
    /// - SQL connection factory for raw SQL queries
    /// - Domain repositories for data access
    /// - Unit of work pattern implementation
    /// </remarks>
    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        // Get the database connection string from configuration
        string? connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        // Register the EF Core DbContext with PostgreSQL provider
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        // Register SQL connection factory for direct database access (non-EF Core queries)
        services.AddSingleton<ISqlConnectionFactory>(_ =>
            new SqlConnectionFactory(connectionString));

        // Register domain repositories for entity access
        services.AddScoped<IUserRepository, UserRepository>();

        // Register UnitOfWork implementation (provided by the DbContext)
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

    /// <summary>
    /// Configures authentication using JWT Bearer tokens and Keycloak integration.
    /// </summary>
    /// <param name="services">The service collection to add the services to.</param>
    /// <param name="configuration">The configuration containing authentication settings.</param>
    /// <remarks>
    /// This method:
    /// - Sets up JWT Bearer as the default authentication scheme
    /// - Configures JWT validation parameters from application settings
    /// - Registers Keycloak integration services
    /// - Sets up HTTP clients for authentication service with proper authorization
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA5404:Do not disable token validation checks", Justification = "<Pending>")]
    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        // Bind application's authentication options from configuration
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication"));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Gọi trực tiếp JwtBearerOptionsSetup từ service provider
                var authOptions = configuration.GetSection("Authentication").Get<AuthenticationOptions>()!;

                var setup = new JwtBearerOptionsSetup(
                    Microsoft.Extensions.Options.Options.Create(authOptions),
                    services.BuildServiceProvider().GetRequiredService<ILogger<JwtBearerOptionsSetup>>()
                );

                setup.Configure(options);
            });

        // Configure Keycloak integration options from configuration
        services.Configure<KeycloakOptions>(configuration.GetSection("Keycloak"));

        // Register handler that adds admin authorization tokens to outgoing requests to Keycloak
        services.AddTransient<AdminAuthorizationDelegatingHandler>();

        // Configure the authentication service with Keycloak integration
        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
        {
            // Get Keycloak settings from configuration
            KeycloakOptions keycloakOptions = serviceProvider
                .GetRequiredService<IOptions<KeycloakOptions>>()
                .Value;

            // Set the base address for all Keycloak admin API requests
            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        })
            // Add the delegating handler that automatically adds admin authentication to Keycloak requests
            .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        // Register the JWT service that handles user authentication via Keycloak
        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            var keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        // Register the HTTP context accessor for user claims access
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
    }
}

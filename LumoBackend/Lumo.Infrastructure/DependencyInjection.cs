using System.Collections;
using Lumo.Application.Abstractions.Clock;
using Lumo.Application.Abstractions.Data;
using Lumo.Application.Abstractions.Email;
using Lumo.Domain.Abstractions;
using Lumo.Domain.Users;
using Lumo.Infrastructure.Clock;
using Lumo.Infrastructure.Data;
using Lumo.Infrastructure.Email;
using Lumo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lumo.Infrastructure;

public static class DependencyInjection
{

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DatetimeProvider>();
        services.AddTransient<IEmailService, EmailService>();

        AddPersistence(services, configuration);

        return services;
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Default") 
            ?? throw new InvalidOperationException("Connection string 'Default' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
        });

        services.AddSingleton<ISqlConnectionFactory>(_ => 
            new SqlConnectionFactory(connectionString));


        // Register other persistence-related services here
        services.AddScoped<IUserRepository, UserRepository>();


        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());   
    }
}


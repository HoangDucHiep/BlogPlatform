using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Lumo.Application;

/// <summary>
/// Provides extension methods for registering application services in the application's service collection.
/// This class handles the registration of MediatR, FluentValidation, and other application-level services.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register MediatR for handling commands and queries
        services.AddMediatR(configuration =>
        {
            // Register all handlers from the current assembly
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


        return services;
    }

}

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Smolla.IdentityProvider.Application.DependencyInjection;

/// <summary>
/// Registers Application-layer services into the host's DI container.
/// </summary>
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddValidatorsFromAssembly(typeof(ApplicationServiceCollectionExtensions).Assembly);

        return services;
    }
}

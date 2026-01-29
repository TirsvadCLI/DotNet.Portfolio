using Microsoft.Extensions.DependencyInjection;

using TirsvadCLI.Portfolio.Infrastructure.Data;

namespace TirsvadCLI.Portfolio.Infrastructure;

/// <summary>
/// Provides extension methods for registering infrastructure-level services in the application's dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services required by the application, such as the Entity Framework Core <see cref="ApplicationDbContext"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to which services are added.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Registers the application's database context for dependency injection.
        _ = services.AddDbContext<ApplicationDbContext>();
        return services;
    }
}

using Microsoft.EntityFrameworkCore;

namespace TirsvadCLI.Portfolio.Infrastructure.Data;

/// <summary>
/// Provides a factory for creating <see cref="DbContextOptions{TContext}"/> instances for <see cref="ApplicationDbContext"/>.
/// </summary>
/// <remarks>
/// This static class centralizes the logic for configuring Entity Framework Core options,
/// supporting environment variable overrides, explicit connection strings, and a default fallback.
/// </remarks>
public static class DbContextOptionsFactory
{
    /// <summary>
    /// Creates a <see cref="DbContextOptions{ApplicationDbContext}"/> instance using the following precedence:
    /// <list type="number">
    ///   <item>
    ///     <description>
    ///       If the environment variable <c>TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION</c> is set and not empty,
    ///       its value is used as the connection string.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       If a <paramref name="connectionString"/> is provided, it is used.
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>
    ///       Otherwise, a default PostgreSQL connection string is used for local development.
    ///     </description>
    ///   </item>
    /// </list>
    /// </summary>
    /// <param name="connectionString">Optional connection string to use if the environment variable is not set.</param>
    /// <returns>A configured <see cref="DbContextOptions{ApplicationDbContext}"/> instance.</returns>
    /// <exception cref="NotSupportedException">
    /// Thrown if called in a context where no connection string is available and the method is not supported.
    /// </exception>
    public static DbContextOptions<ApplicationDbContext> Create(string? connectionString = default)
    {
        string? envConnectionString = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        return !string.IsNullOrEmpty(envConnectionString)
            ? OptionsBuilder(envConnectionString)
            : connectionString is not null
                ? OptionsBuilder(connectionString)
                : OptionsBuilder("Host=localhost;Port=5434;Database=portfolio;Username=postgres;Password=postgres");
        throw new NotSupportedException("DbContextOptionsFactory.Create() is only supported in DEBUG builds.");
    }

    /// <summary>
    /// Configures and returns <see cref="DbContextOptions{ApplicationDbContext}"/> for PostgreSQL using the provided connection string.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    /// <returns>A configured <see cref="DbContextOptions{ApplicationDbContext}"/> instance.</returns>
    private static DbContextOptions<ApplicationDbContext> OptionsBuilder(string connectionString)
    {
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
        _ = optionsBuilder.UseNpgsql(connectionString);
        return optionsBuilder.Options;
    }
}

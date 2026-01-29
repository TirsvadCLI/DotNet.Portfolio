using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TirsvadCLI.Portfolio.Infrastructure.Data;

/// <summary>
/// Factory for creating <see cref="ApplicationDbContext"/> instances at design time.
/// <para>
/// Used by Entity Framework Core tools (e.g., for migrations) to instantiate the DbContext
/// when the application is not running. It retrieves the connection string from the
/// <c>TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION</c> environment variable,
/// or falls back to a default local PostgreSQL connection string if not set.
/// </para>
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    /// <summary>
    /// Creates a new <see cref="ApplicationDbContext"/> instance using the appropriate connection string.
    /// </summary>
    /// <param name="args">Command-line arguments (not used).</param>
    /// <returns>A configured <see cref="ApplicationDbContext"/> instance.</returns>
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        string? envConnectionString = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        if (string.IsNullOrWhiteSpace(envConnectionString))
        {
            envConnectionString = "Host=localhost;Port=5434;Database=portfolio;Username=postgres;Password=postgres";
        }
        DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder = new();
        _ = optionsBuilder.UseNpgsql(envConnectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}


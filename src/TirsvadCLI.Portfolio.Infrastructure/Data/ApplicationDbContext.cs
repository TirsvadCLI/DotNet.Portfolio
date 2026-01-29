using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore;

using TirsvadCLI.Portfolio.Core.Abstracts;
using TirsvadCLI.Portfolio.Domain.Entities;
using TirsvadCLI.Portfolio.Infrastructure.Data.Configurations;

namespace TirsvadCLI.Portfolio.Infrastructure.Data;

/// <summary>
///     Provides access to ASP.NET Core Identity and application-specific entities using Entity Framework Core.
///     Inherits from <see cref="IdentityDbContext{TUser, TRole, TKey}"/> with <see cref="ApplicationUser"/> and <see cref="IdentityRole{Guid}"/>.
///     Configures the database context for the TirsvadCLI Portfolio application.
/// </summary>
/// <remarks>
///     This context is responsible for managing the database connection, entity sets, and model configuration.
///     It applies custom entity configurations and supports dependency injection via <see cref="DbContextOptions{TContext}"/>.
/// </remarks>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    #region constructors
    /// <summary>
    ///     Initializes a new instance of the <see cref="ApplicationDbContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the <see cref="DbContext"/>.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
#if DEBUG
        //Database.EnsureDeleted();
        //Database.EnsureCreated();
#endif
    }

    #endregion

    #region overrides
    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
    }
    #endregion

}

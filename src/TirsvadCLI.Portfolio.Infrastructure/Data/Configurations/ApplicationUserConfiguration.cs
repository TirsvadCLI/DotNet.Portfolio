using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TirsvadCLI.Portfolio.Domain.Entities;
using TirsvadCLI.Portfolio.Infrastructure.Data.Configurations.SeedsDevelopment;

namespace TirsvadCLI.Portfolio.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for the <see cref="ApplicationUser"/> entity.
/// </summary>
/// <remarks>
/// <para>
/// This configuration class is responsible for setting up the <see cref="ApplicationUser"/> entity within the EF Core model.
/// In <c>DEBUG</c> builds, it seeds a test user for development and testing purposes by invoking <see cref="SeedTestIfNeeded.SeedTestUser"/>.
/// </para>
/// <para>
/// Usage:
/// <list type="bullet">
///   <item>
///     <description>Applied automatically by EF Core when building the model via <c>OnModelCreating</c>.</description>
///   </item>
///   <item>
///     <description>Seeding is only performed in development/debug environments to avoid test data in production.</description>
///   </item>
/// </list>
/// </para>
/// <para>
/// See also:
/// <list type="bullet">
///   <item>
///     <description><see cref="IEntityTypeConfiguration{TEntity}"/></description>
///   </item>
///   <item>
///     <description><see cref="SeedTestIfNeeded"/></description>
///   </item>
/// </list>
/// </para>
/// </remarks>
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    /// <summary>
    /// Configures the <see cref="ApplicationUser"/> entity for Entity Framework Core.
    /// In DEBUG builds, seeds a test user for development and testing purposes.
    /// </summary>
    /// <param name="builder">The builder used to configure the <see cref="ApplicationUser"/> entity.</param>
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
#if DEBUG
        // Seeds a test user entity when running in DEBUG mode.
        SeedTestIfNeeded.SeedTestUser(builder);
#endif
    }
}

using Microsoft.EntityFrameworkCore.Metadata.Builders;

using TirsvadCLI.Portfolio.Domain.Entities;

namespace TirsvadCLI.Portfolio.Infrastructure.Data.Configurations.SeedsDevelopment;

/// <summary>
/// Provides development-time seeding functionality for test users in the database.
/// </summary>
public class SeedTestIfNeeded
{
    /// <summary>
    /// Seeds a test <see cref="ApplicationUser"/> entity into the database using the provided <see cref="EntityTypeBuilder{TEntity}"/>.
    /// <para>
    /// The seeded user has the following properties:
    /// <list type="bullet">
    /// <item><description>Id: 11111111-1111-1111-1111-111111111111</description></item>
    /// <item><description>UserName: "testuser"</description></item>
    /// <item><description>Email: "testuser@example.com"</description></item>
    /// <item><description>EmailConfirmed: true</description></item>
    /// <item><description>PasswordHash: Precomputed for password 'Test@1234' using ASP.NET Core Identity v7+</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Note:</b> If the password or user properties are changed, the password hash must be recomputed using the same Identity version.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="EntityTypeBuilder{ApplicationUser}"/> used to configure the entity and seed data.</param>
    public static void SeedTestUser(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Precomputed hash for password 'Test@1234' using ASP.NET Core Identity v7+ (for ApplicationUser)
        // If you change the password, recompute the hash using the same Identity version and user properties.
        ApplicationUser user = new()
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserName = "testuser",
            NormalizedUserName = "TESTUSER",
            Email = "testuser@example.com",
            NormalizedEmail = "TESTUSER@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = "11111111-1111-1111-1111-111111111112",
            ConcurrencyStamp = "11111111-1111-1111-1111-111111111113",
            PasswordHash = "AQAAAAEAACcQAAAAEJQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQwQw==" // Hash for password 'Test1234#'
        };

        _ = builder.HasData(user);
    }
}

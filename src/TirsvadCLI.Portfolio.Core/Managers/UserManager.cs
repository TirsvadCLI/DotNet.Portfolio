using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;

using TirsvadCLI.Portfolio.Core.Abstracts;
using TirsvadCLI.Portfolio.Domain.Entities;
namespace TirsvadCLI.Portfolio.Core.Managers;

/// <summary>
/// Provides user management operations for the application, including user retrieval, password verification, and user creation.
/// </summary>
/// <remarks>
/// <para>
/// <b>Responsibilities:</b>
/// <list type="bullet">
///   <item>Find users by username.</item>
///   <item>Verify user passwords using PBKDF2 hashing with HMACSHA256.</item>
///   <item>Add new users to the database context.</item>
/// </list>
/// </para>
/// <para>
/// <b>Dependencies:</b>
/// <list type="bullet">
///   <item><see cref="IApplicationDbContext"/>: Used for data access to user entities.</item>
///   <item>Relies on EF Core for database operations.</item>
/// </list>
/// </para>
/// <para>
/// <b>Password Hashing:</b>
/// Passwords are hashed using PBKDF2 with HMACSHA256, 10,000 iterations, and a 256-bit key. The hash is stored as "base64Salt:base64Hash".
/// </para>
/// <para>
/// <b>Thread Safety:</b>
/// This class is stateless except for the injected <see cref="IApplicationDbContext"/>.
/// </para>
/// <para>
/// <b>References:</b>
/// <see href="https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-10.0"/>
/// </para>
/// </remarks>
public class UserManager
{
    private readonly Func<IApplicationDbContext> _dbContextFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserManager"/> class with the specified database context factory.
    /// </summary>
    /// <param name="dbContextFactory">A factory for creating application database contexts.</param>
    public UserManager(Func<IApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    /// <summary>
    /// Asynchronously finds a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The <see cref="ApplicationUser"/> if found; otherwise, <c>null</c>.</returns>
    public async Task<ApplicationUser?> FindByNameAsync(string username)
    {
        IApplicationDbContext context = _dbContextFactory();
        try
        {
            return await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }
        finally
        {
            (context as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Verifies a user's password against the stored password hash.
    /// </summary>
    /// <param name="user">The user whose password is being checked.</param>
    /// <param name="password">The plaintext password to verify.</param>
    /// <returns><c>true</c> if the password matches; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Assumes <see cref="ApplicationUser.PasswordHash"/> is stored as "base64Salt:base64Hash".
    /// </remarks>
    public static Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        // Assumes PasswordHash is stored as "base64Salt:base64Hash"
        if (string.IsNullOrEmpty(user.PasswordHash) || !user.PasswordHash.Contains(':'))
            return Task.FromResult(false);

        string[] parts = user.PasswordHash.Split(':');
        if (parts.Length != 2)
            return Task.FromResult(false);

        byte[] saltBytes = Convert.FromBase64String(parts[0]);
        string storedHash = parts[1];

        string hash = HashPassword(password, saltBytes);
        return Task.FromResult(storedHash == hash);
    }

    /// <summary>
    /// Asynchronously adds a new user to the database.
    /// </summary>
    /// <param name="user">The user entity to add.</param>
    public async Task AddUserAsync(ApplicationUser user)
    {
        IApplicationDbContext context = _dbContextFactory();
        try
        {
            _ = context.Users.Add(user);
            _ = await context.SaveChangesAsync();
        }
        finally
        {
            (context as IDisposable)?.Dispose();
        }
    }

    /// <summary>
    /// Hashes a password using PBKDF2 with HMACSHA256.
    /// </summary>
    /// <param name="password">The plaintext password.</param>
    /// <param name="salt">The salt bytes.</param>
    /// <returns>The base64-encoded password hash.</returns>
    /// <remarks>
    /// Reference: https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing?view=aspnetcore-10.0
    /// </remarks>
    private static string HashPassword(string password, byte[] salt)
    {
        return Convert.ToBase64String(
            KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
    }
}

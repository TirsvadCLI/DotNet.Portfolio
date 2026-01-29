namespace TirsvadCLI.Portfolio.Core.Abstracts;

public interface IAuthenticationService
{
    /// <summary>
    /// Authenticates a user and returns a JWT token if successful, otherwise null.
    /// </summary>
    /// <param name="username">The username.</param>
    /// <param name="password">The password.</param>
    /// <returns>JWT token string if authentication is successful; otherwise, null.</returns>
    Task<string?> AuthenticateAsync(string username, string password);
}

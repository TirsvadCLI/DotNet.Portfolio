
using System.Threading.Tasks;
using TirsvadCLI.Portfolio.Core.Abstracts;
using TirsvadCLI.Portfolio.Core.Managers;

namespace TirsvadCLI.Portfolio.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly SignInManager _signInManager;

    public AuthenticationService(SignInManager signInManager)
    {
        _signInManager = signInManager;
    }

    public Task<string?> AuthenticateAsync(string username, string password)
        => _signInManager.AuthenticateAsync(username, password);
}

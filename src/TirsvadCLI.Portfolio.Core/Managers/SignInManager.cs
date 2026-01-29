using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using TirsvadCLI.Portfolio.Domain.Entities;

namespace TirsvadCLI.Portfolio.Core.Managers;

public class SignInManager
{
    #region Fields

    private readonly UserManager _userManager;
    private readonly string _jwtKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;

    #endregion

    #region Properties
    // No properties defined
    #endregion

    #region Public Methods

    public SignInManager(UserManager userManager, string jwtKey, string jwtIssuer, string jwtAudience)
    {
        _userManager = userManager;
        _jwtKey = jwtKey;
        _jwtIssuer = jwtIssuer;
        _jwtAudience = jwtAudience;
    }

    public async Task<string?> AuthenticateAsync(string username, string password)
    {
        ApplicationUser? user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return null;

        bool valid = await UserManager.CheckPasswordAsync(user, password);
        if (!valid)
            return null;

        // Generate JWT
        JwtSecurityTokenHandler tokenHandler = new();
        byte[] key = Encoding.UTF8.GetBytes(_jwtKey);
        Claim[] claims =
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    #endregion

    #region Events
    // No events defined
    #endregion

    #region CanDoXXX Commands
    // No CanDoXXX commands defined
    #endregion

    #region OnXXX Commands
    // No OnXXX commands defined
    #endregion

    #region Helpers
    // No helpers defined
    #endregion
}

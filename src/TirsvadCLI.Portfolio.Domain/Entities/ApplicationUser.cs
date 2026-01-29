using Microsoft.AspNetCore.Identity;

using TirsvadCLI.Portfolio.Domain.Abstracts;

namespace TirsvadCLI.Portfolio.Domain.Entities;

/// <summary>
/// Represents an application user entity for authentication and authorization.
/// Inherits from <see cref="IdentityUser{Guid}"/> to integrate with ASP.NET Core Identity using a <see cref="Guid"/> as the primary key.
/// Implements <see cref="IEntity"/> to ensure consistency across domain entities.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>, IEntity
{
}

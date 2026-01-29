namespace TirsvadCLI.Portfolio.Domain.Abstracts;

/// <summary>
/// Represents a base contract for all domain entities in the system.
/// </summary>
/// <remarks>
/// All domain entities should implement this interface to ensure they have a unique identifier.
/// </remarks>
public interface IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    Guid Id { get; set; }
}

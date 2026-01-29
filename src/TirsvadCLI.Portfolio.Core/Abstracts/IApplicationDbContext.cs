using Microsoft.EntityFrameworkCore;

using TirsvadCLI.Portfolio.Domain.Entities;

namespace TirsvadCLI.Portfolio.Core.Abstracts;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

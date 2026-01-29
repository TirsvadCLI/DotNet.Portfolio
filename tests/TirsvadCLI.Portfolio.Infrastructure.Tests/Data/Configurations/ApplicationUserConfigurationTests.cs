using TirsvadCLI.Portfolio.Infrastructure.Data;

namespace TirsvadCLI.Portfolio.Infrastructure.Tests.Data.Configurations;

public class ApplicationDbContextIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbContextIntegrationTests()
    {
        _dbContext = new ApplicationDbContext(TirsvadCLI.Portfolio.Infrastructure.Data.DbContextOptionsFactory.Create());
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }
}

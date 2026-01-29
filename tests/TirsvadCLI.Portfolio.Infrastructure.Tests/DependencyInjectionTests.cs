using Microsoft.Extensions.DependencyInjection;

namespace TirsvadCLI.Portfolio.Infrastructure.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddInfrastructureServices_ReturnsSameServiceCollection()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        IServiceCollection result = services.AddInfrastructureServices();

        // Assert
        Assert.Same(services, result);
    }

}

using Microsoft.Extensions.DependencyInjection;

namespace TirsvadCLI.Portfolio.Core.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddCoreServices_ReturnsSameServiceCollectionInstance()
    {
        // Arrange
        ServiceCollection services = new();

        // Act
        IServiceCollection result = services.AddCoreServices();

        // Assert
        Assert.Same(services, result);
    }
}

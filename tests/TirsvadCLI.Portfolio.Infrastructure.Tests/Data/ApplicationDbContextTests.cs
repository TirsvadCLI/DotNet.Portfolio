using Microsoft.EntityFrameworkCore.Metadata;

using TirsvadCLI.Portfolio.Infrastructure.Data;

namespace TirsvadCLI.Portfolio.Infrastructure.Tests.Data;

public class ApplicationDbContextTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;

    public ApplicationDbContextTests()
    {
        _dbContext = new ApplicationDbContext(DbContextOptionsFactory.Create());
    }

    public void Dispose()
    {
        _dbContext?.Dispose();
    }


    [Fact]
    [Trait("Category", "Functional")]
    public void CanConstructDbContext()
    {
        Assert.NotNull(_dbContext);
    }

    [Fact]
    [Trait("Category", "Functional")]
    public void CanCallOnModelCreating()
    {
        // This will trigger OnModelCreating via Model property
        IModel model = _dbContext.Model;
        Assert.NotNull(model);
    }

    //[Fact]
    //[Trait("Category", "Functional")]
    //public void CanConnectToDatabase()
    //{
    //    using var dbContext = new ApplicationDbContext(DbContextOptionsFactory.Create());
    //    var canConnect = dbContext.Database.CanConnect();
    //    Assert.True(canConnect, "Database connection should succeed.");
    //}

    //[Fact]
    //[Trait("Category", "Concurrency")]
    //public void DbContext_IsNotThreadSafe_SingleInstanceThrows()
    //{
    //    Exception? threadException = null;
    //    Thread thread = new(() =>
    //    {
    //        try
    //        {
    //            // Accessing DbContext from another thread should throw
    //            IModel _ = _dbContext.Model;
    //        }
    //        catch (Exception ex)
    //        {
    //            threadException = ex;
    //        }
    //    });
    //    thread.Start();
    //    thread.Join();
    //    Assert.NotNull(threadException);
    //}

    [Fact]
    [Trait("Category", "Concurrency")]
    public void DbContext_IsThreadSafe_PerInstancePerThread()
    {
        Exception? threadException = null;
        Thread thread = new(() =>
        {
            try
            {
                using ApplicationDbContext dbContext = new(DbContextOptionsFactory.Create());
                IModel _ = dbContext.Model;
            }
            catch (Exception ex)
            {
                threadException = ex;
            }
        });
        thread.Start();
        thread.Join();
        Assert.Null(threadException);
    }
}

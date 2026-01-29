using Npgsql;

using System.Collections.Concurrent;
using System.Diagnostics;

using TirsvadCLI.Portfolio.Infrastructure.Data;

namespace TirsvadCLI.Portfolio.Infrastructure.Tests.Data;

public class DesignTimeDbContextFactoryTests : IDisposable
{
    private const string _testDatabasePrefix = "portfolio_test_";
    private string[] _createdDatabases = [];

    [Fact(DisplayName = "CreateDbContext returns valid context for real PostgreSQL test DB")]
    [Trait("Category", "Functional")]
    public void CreateDbContext_ReturnsValidContext()
    {
        // Arrange
        string dbName = $"{_testDatabasePrefix}{Guid.NewGuid():N}";
        _createdDatabases = [.. _createdDatabases, dbName];
        string testDbConnStr = ConnectionStringHelper.GetDefaultConnectionString(dbName);
        string adminConnStr = testDbConnStr.Replace($"Database={dbName}", "Database=postgres");
        using NpgsqlConnection adminConn = new(adminConnStr);
        adminConn.Open();
        using (NpgsqlCommand cmd = adminConn.CreateCommand())
        {
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
        // Set environment variable so factory uses the test DB
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", testDbConnStr);
        try
        {
            DesignTimeDbContextFactory factory = new();
            using ApplicationDbContext context = factory.CreateDbContext([]);
            _ = context.Database.EnsureCreated();
            Assert.True(context.Database.CanConnect());
        }
        finally
        {
            // Drop test DB
            adminConn.Close();
            using NpgsqlConnection dropConn = new(adminConnStr);
            dropConn.Open();
            using NpgsqlCommand cmd = dropConn.CreateCommand();
            cmd.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
            _ = cmd.ExecuteNonQuery();
            cmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
    }

    [Fact(DisplayName = "CreateDbContext returns false on connection error (COM error)")]
    [Trait("Category", "Functional")]
    public void CreateDbContext_ReturnsFalseOnConnectionError()
    {
        string? origConnStr = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        // Arrange
        string connStr = "Host=localhost;Port=9999;Database=invalid;Username=invalid;Password=invalid";
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", connStr);
        DesignTimeDbContextFactory factory = new();
        // Act
        using ApplicationDbContext context = factory.CreateDbContext([]);
        // Assert
        Assert.False(context.Database.CanConnect());
        // Cleanup
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", origConnStr);
    }

    [Fact(DisplayName = "CreateDbContext is thread-safe for parallel creation")]
    [Trait("Category", "Concurrency")]
    public void CreateDbContext_IsThreadSafe()
    {
        string dbName = $"{_testDatabasePrefix}{Guid.NewGuid():N}";
        _createdDatabases = [.. _createdDatabases, dbName];
        string connStr = ConnectionStringHelper.GetDefaultConnectionString(dbName);
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", connStr);
        using NpgsqlConnection adminConn = new(connStr.Replace($"Database={dbName}", "Database=postgres"));
        adminConn.Open();
        using (NpgsqlCommand cmd = adminConn.CreateCommand())
        {
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
        try
        {
            DesignTimeDbContextFactory factory = new();
            ConcurrentBag<Exception> exceptions = [];
            _ = Parallel.For(0, 10, _ =>
            {
                try
                {
                    using ApplicationDbContext context = factory.CreateDbContext([]);
                    bool result = context.Database.EnsureCreated();
                    Assert.True(context.Database.CanConnect());
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            });
            Assert.Empty(exceptions);
        }
        finally
        {
            adminConn.Close();
            using NpgsqlConnection dropConn = new(connStr.Replace($"Database={dbName}", "Database=postgres"));
            dropConn.Open();
            using NpgsqlCommand cmd = dropConn.CreateCommand();
            cmd.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
            _ = cmd.ExecuteNonQuery();
            cmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
    }

    [Fact(DisplayName = "CreateDbContext supports multi-session usage")]
    [Trait("Category", "Concurrency")]
    public void CreateDbContext_MultiSession()
    {
        string dbName = $"{_testDatabasePrefix}{Guid.NewGuid():N}";
        _createdDatabases = [.. _createdDatabases, dbName];
        string connStr = ConnectionStringHelper.GetDefaultConnectionString(dbName);
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", connStr);
        using NpgsqlConnection adminConn = new(connStr.Replace($"Database={dbName}", "Database=postgres"));
        adminConn.Open();
        using (NpgsqlCommand cmd = adminConn.CreateCommand())
        {
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
        try
        {
            DesignTimeDbContextFactory factory = new();
            using ApplicationDbContext context1 = factory.CreateDbContext([]);
            using ApplicationDbContext context2 = factory.CreateDbContext([]);
            _ = context1.Database.EnsureCreated();
            _ = context2.Database.EnsureCreated();
            Assert.True(context1.Database.CanConnect());
            Assert.True(context2.Database.CanConnect());
        }
        finally
        {
            adminConn.Close();
            using NpgsqlConnection dropConn = new(connStr.Replace($"Database={dbName}", "Database=postgres"));
            dropConn.Open();
            using NpgsqlCommand cmd = dropConn.CreateCommand();
            cmd.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
            _ = cmd.ExecuteNonQuery();
            cmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
    }

    [Fact(DisplayName = "CreateDbContext performance benchmark")]
    [Trait("Category", "Benchmark")]
    public void CreateDbContext_Performance()
    {
        string dbName = $"{_testDatabasePrefix}{Guid.NewGuid():N}";
        _createdDatabases = [.. _createdDatabases, dbName];
        string connStr = ConnectionStringHelper.GetDefaultConnectionString(dbName);
        //_ = $"Host=localhost;Port=5434;Database={dbName};Username=postgres;Password=postgres";
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", connStr);
        using NpgsqlConnection adminConn = new(connStr.Replace($"Database={dbName}", "Database=postgres"));
        adminConn.Open();
        using (NpgsqlCommand cmd = adminConn.CreateCommand())
        {
            cmd.CommandText = $"CREATE DATABASE \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
        try
        {
            // Ensure schema is created once before benchmarking
            DesignTimeDbContextFactory factory = new();
            using (ApplicationDbContext setupContext = factory.CreateDbContext([]))
            {
                _ = setupContext.Database.EnsureCreated();
            }
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                using ApplicationDbContext context = factory.CreateDbContext([]);
                Assert.True(context.Database.CanConnect());
            }
            sw.Stop();
            Assert.InRange(sw.ElapsedMilliseconds, 0, 5000); // 10 contexts in under 5 seconds
        }
        finally
        {
            adminConn.Close();
            using NpgsqlConnection dropConn = new(connStr.Replace($"Database={dbName}", "Database=postgres"));
            dropConn.Open();
            using NpgsqlCommand cmd = dropConn.CreateCommand();
            cmd.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
            _ = cmd.ExecuteNonQuery();
            cmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\";";
            _ = cmd.ExecuteNonQuery();
        }
    }

    public void Dispose()
    {
        foreach (string dbName in _createdDatabases)
        {
            if (!dbName.StartsWith(_testDatabasePrefix))
                continue;
            try
            {
                string adminConnStr = ConnectionStringHelper.GetDefaultConnectionString(dbName)
                    .Replace($"Database={dbName}", "Database=postgres");
                using NpgsqlConnection conn = new(adminConnStr);
                conn.Open();
                using NpgsqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{dbName}';";
                _ = cmd.ExecuteNonQuery();
                cmd.CommandText = $"DROP DATABASE IF EXISTS \"{dbName}\";";
                _ = cmd.ExecuteNonQuery();
            }
            catch
            {
                // Ignore errors in destructor
            }
        }
        GC.SuppressFinalize(this);
    }
}

public static class ConnectionStringHelper
{
    public static string GetDefaultConnectionString(string databaseName)
    {
        string? envConnectionString = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        if (string.IsNullOrWhiteSpace(envConnectionString))
        {
            envConnectionString = "Host=localhost;Port=5434;Database=portfolio;Username=postgres;Password=postgres";
        }

        // Replace or add the Database part
        NpgsqlConnectionStringBuilder builder = new(envConnectionString)
        {
            Database = databaseName
        };
        return builder.ConnectionString;
    }
}

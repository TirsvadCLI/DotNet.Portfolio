using Microsoft.EntityFrameworkCore;

using Npgsql;

using System.Diagnostics;
using System.Reflection;

using TirsvadCLI.Portfolio.Core.Abstracts;
using TirsvadCLI.Portfolio.Core.Managers;
using TirsvadCLI.Portfolio.Domain.Entities;
using TirsvadCLI.Portfolio.Infrastructure.Data;

namespace TirsvadCLI.Portfolio.Core.Tests.Managers;

[Collection("Database collection")]
public class UserManagerTests : IDisposable
{
    private readonly string _dbName;
    private readonly string _connStr;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager _userManager;

    private readonly Func<IApplicationDbContext> _dbContextFactory;

    public UserManagerTests()
    {
        _dbName = $"portfolio_test_{Guid.NewGuid():N}";
        _connStr = ConnectionStringHelper.GetDefaultConnectionString(_dbName);
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", _connStr);
        DesignTimeDbContextFactory factory = new();
        _dbContext = factory.CreateDbContext(Array.Empty<string>());
        _ = _dbContext.Database.EnsureCreated();
        _dbContextFactory = () => new DesignTimeDbContextFactory().CreateDbContext(Array.Empty<string>());
        _userManager = new UserManager(_dbContextFactory);
    }

    [Fact(DisplayName = "AddUserAsync and FindByNameAsync work"), Trait("Category", "Functional")]
    public async Task AddUserAndFindByNameAsync_Works()
    {
        ApplicationUser user = new() { UserName = "testuser", Email = "test@example.com" };
        await _userManager.AddUserAsync(user);
        ApplicationUser? found = await _userManager.FindByNameAsync("testuser");
        Assert.NotNull(found);
        Assert.Equal("testuser", found!.UserName);
    }

    [Fact(DisplayName = "CheckPasswordAsync returns true for correct password"), Trait("Category", "Functional")]
    public async Task CheckPasswordAsync_CorrectPassword_ReturnsTrue()
    {
        byte[] salt = Guid.NewGuid().ToByteArray();
        string password = "P@ssw0rd!";
        string hash = InvokeHashPassword(password, salt);
        ApplicationUser user = new() { UserName = "pwuser", PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}" };
        bool result = await UserManager.CheckPasswordAsync(user, password);
        Assert.True(result);
    }

    [Fact(DisplayName = "CheckPasswordAsync returns false for wrong password"), Trait("Category", "Functional")]
    public async Task CheckPasswordAsync_WrongPassword_ReturnsFalse()
    {
        byte[] salt = Guid.NewGuid().ToByteArray();
        string hash = InvokeHashPassword("right", salt);
        ApplicationUser user = new() { UserName = "pwuser2", PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}" };
        bool result = await UserManager.CheckPasswordAsync(user, "wrong");
        Assert.False(result);
    }

    [Fact(DisplayName = "FindByNameAsync returns null for missing user"), Trait("Category", "Functional")]
    public async Task FindByNameAsync_ReturnsNullForMissing()
    {
        ApplicationUser? found = await _userManager.FindByNameAsync("notfound");
        Assert.Null(found);
    }

    [Fact(DisplayName = "AddUserAsync throws on COM/connection error"), Trait("Category", "Functional")]
    public async Task AddUserAsync_ThrowsOnComError()
    {
        string? origEnv = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        // Use an invalid connection string
        string badConnStr = "Host=localhost;Port=9999;Database=invalid;Username=invalid;Password=invalid";
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", badConnStr);
        try
        {
            DesignTimeDbContextFactory factory = new();
            Func<TirsvadCLI.Portfolio.Core.Abstracts.IApplicationDbContext> badFactory = () => factory.CreateDbContext(Array.Empty<string>());
            UserManager badManager = new(badFactory);
            ApplicationUser user = new() { UserName = "failuser" };
            _ = await Assert.ThrowsAnyAsync<Exception>(() => badManager.AddUserAsync(user));
        }
        finally
        {
            Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", origEnv);
        }
    }

    [Fact(DisplayName = "UserManager is thread-safe for concurrent FindByNameAsync"), Trait("Category", "Concurrency")]
    public async Task UserManager_ThreadSafe_FindByNameAsync()
    {
        ApplicationUser user = new() { UserName = "threaduser", Email = "thread@example.com" };
        await _userManager.AddUserAsync(user);
        IEnumerable<Task<ApplicationUser?>> tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(async () => await _userManager.FindByNameAsync("threaduser")));
        ApplicationUser?[] results = await Task.WhenAll(tasks);
        Assert.All(results, Assert.NotNull);
    }

    [Fact(DisplayName = "UserManager supports multi-session (multiple contexts)"), Trait("Category", "Concurrency")]
    public async Task UserManager_MultiSession_Support()
    {
        ApplicationUser user = new() { UserName = "multiuser", Email = "multi@example.com" };
        await _userManager.AddUserAsync(user);
        DesignTimeDbContextFactory factory = new();
        List<ApplicationDbContext> contexts = Enumerable.Range(0, 3).Select(_ => factory.CreateDbContext(Array.Empty<string>())).ToList();
        try
        {
            IEnumerable<Task<ApplicationUser?>> tasks = contexts.Select(ctx => Task.Run(async () =>
            {
                UserManager mgr = new(() => ctx);
                return await mgr.FindByNameAsync("multiuser");
            }));
            ApplicationUser?[] results = await Task.WhenAll(tasks);
            Assert.All(results, Assert.NotNull);
        }
        finally
        {
            foreach (ApplicationDbContext ctx in contexts) ctx.Dispose();
        }
    }

    [Fact(DisplayName = "HashPassword performance benchmark"), Trait("Category", "Benchmark")]
    public void HashPassword_Performance_Benchmark()
    {
        byte[] salt = Guid.NewGuid().ToByteArray();
        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 100; i++)
        {
            _ = InvokeHashPassword($"pw{i}", salt);
        }
        sw.Stop();
        Assert.InRange(sw.ElapsedMilliseconds, 0, 2000); // Should be fast enough for 100 hashes
    }

    [Fact(DisplayName = "HashPassword produces deterministic output"), Trait("Category", "Functional")]
    public void HashPassword_Deterministic()
    {
        byte[] salt = Guid.NewGuid().ToByteArray();
        string pw = "deterministic";
        string h1 = InvokeHashPassword(pw, salt);
        string h2 = InvokeHashPassword(pw, salt);
        Assert.Equal(h1, h2);
    }

    private static string InvokeHashPassword(string password, byte[] salt)
    {
        MethodInfo method = typeof(UserManager).GetMethod("HashPassword", BindingFlags.NonPublic | BindingFlags.Static)!;
        return (string)method.Invoke(null, new object[] { password, salt })!;
    }

    public void Dispose()
    {
        // Drop the test DB
        string adminConnStr = _connStr.Replace($"Database={_dbName}", "Database=postgres");
        using NpgsqlConnection conn = new(adminConnStr);
        conn.Open();
        using NpgsqlCommand cmd = conn.CreateCommand();
        cmd.CommandText = $"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{_dbName}';";
        _ = cmd.ExecuteNonQuery();
        cmd.CommandText = $"DROP DATABASE IF EXISTS \"{_dbName}\";";
        _ = cmd.ExecuteNonQuery();
    }
}

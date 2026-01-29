using Npgsql;

using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;

using TirsvadCLI.Portfolio.Core.Abstracts;

//using System.Diagnostics;
//using System.IdentityModel.Tokens.Jwt;
//using System.Reflection;

//using TirsvadCLI.Portfolio.Core.Abstracts;
using TirsvadCLI.Portfolio.Core.Managers;
using TirsvadCLI.Portfolio.Domain.Entities;

//using TirsvadCLI.Portfolio.Domain.Entities;
using TirsvadCLI.Portfolio.Infrastructure.Data;

namespace TirsvadCLI.Portfolio.Core.Tests.Managers;

// Ensures SignInManagerTests waits for UserManagerTests to finish
[Collection("Database collection")]
public class SignInManagerTests : IDisposable
{
    private readonly string _dbName;
    private readonly string _connStr;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager _userManager;
    private readonly SignInManager _signInManager;
    private readonly string _jwtKey = "test_jwt_key_12345678901234567890123456789012";
    private readonly string _jwtIssuer = "test-issuer";
    private readonly string _jwtAudience = "test-audience";
    private readonly Func<IApplicationDbContext> _dbContextFactory;

    public SignInManagerTests()
    {
        _dbName = $"portfolio_test_{Guid.NewGuid():N}";
        _connStr = ConnectionStringHelper.GetDefaultConnectionString(_dbName);
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", _connStr);
        DesignTimeDbContextFactory factory = new();
        _dbContext = factory.CreateDbContext(Array.Empty<string>());
        _ = _dbContext.Database.EnsureCreated();
        _dbContextFactory = () => new DesignTimeDbContextFactory().CreateDbContext(Array.Empty<string>());

        _userManager = new UserManager(_dbContextFactory);
        _signInManager = new SignInManager(_userManager, _jwtKey, _jwtIssuer, _jwtAudience);
    }

    [Fact(DisplayName = "AuthenticateAsync returns JWT for valid user/password"), Trait("Category", "Functional")]
    public async Task AuthenticateAsync_ValidUser_ReturnsJwt()
    {
        ApplicationUser user = new() { UserName = "signinuser", Email = "signin@example.com" };
        await _userManager.AddUserAsync(user);
        // Reload user from current context to ensure tracking
        user = _dbContext.Users.First(u => u.UserName == "signinuser");
        // Set password
        byte[] salt = Guid.NewGuid().ToByteArray();
        string hash = InvokeHashPassword("secret", salt);
        user.PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}";
        _ = _dbContext.Users.Update(user);
        _ = await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string? jwt = await _signInManager.AuthenticateAsync("signinuser", "secret");
        Assert.False(string.IsNullOrWhiteSpace(jwt));
        // Validate JWT structure
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken token = handler.ReadJwtToken(jwt);
        Assert.Equal(_jwtIssuer, token.Issuer);
        Assert.Equal(_jwtAudience, token.Audiences.First());
    }

    [Fact(DisplayName = "AuthenticateAsync returns null for wrong password"), Trait("Category", "Functional")]
    public async Task AuthenticateAsync_WrongPassword_ReturnsNull()
    {
        ApplicationUser user = new() { UserName = "wrongpwuser", Email = "wrongpw@example.com" };
        await _userManager.AddUserAsync(user);
        byte[] salt = Guid.NewGuid().ToByteArray();
        string hash = InvokeHashPassword("rightpw", salt);
        user.PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}";
        _ = _dbContext.Users.Update(user);
        _ = await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        string? jwt = await _signInManager.AuthenticateAsync("wrongpwuser", "wrongpw");
        Assert.Null(jwt);
    }

    [Fact(DisplayName = "AuthenticateAsync returns null for missing user"), Trait("Category", "Functional")]
    public async Task AuthenticateAsync_MissingUser_ReturnsNull()
    {
        string? jwt = await _signInManager.AuthenticateAsync("nouser", "nopw");
        Assert.Null(jwt);
    }

    [Fact(DisplayName = "AuthenticateAsync throws on COM/connection error"), Trait("Category", "Functional")]
    public async Task AuthenticateAsync_ThrowsOnComError()
    {
        string? origEnv = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        string badConnStr = "Host=localhost;Port=9999;Database=invalid;Username=invalid;Password=invalid";
        Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", badConnStr);
        try
        {
            DesignTimeDbContextFactory factory = new();
            Func<TirsvadCLI.Portfolio.Core.Abstracts.IApplicationDbContext> badFactory = () => factory.CreateDbContext(Array.Empty<string>());
            UserManager badManager = new(badFactory);
            SignInManager badSignIn = new(badManager, _jwtKey, _jwtIssuer, _jwtAudience);
            _ = await Assert.ThrowsAnyAsync<Exception>(() => badSignIn.AuthenticateAsync("failuser", "failpw"));
        }
        finally
        {
            Environment.SetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION", origEnv);
        }
    }

    [Fact(DisplayName = "SignInManager is thread-safe for concurrent AuthenticateAsync"), Trait("Category", "Concurrency")]
    public async Task SignInManager_ThreadSafe_AuthenticateAsync()
    {
        ApplicationUser user = new() { UserName = "threadsignin", Email = "threadsignin@example.com" };
        await _userManager.AddUserAsync(user);
        byte[] salt = Guid.NewGuid().ToByteArray();
        string hash = InvokeHashPassword("threadpw", salt);
        user.PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}";
        _ = _dbContext.Users.Update(user);
        _ = await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        IEnumerable<Task<string?>> tasks = Enumerable.Range(0, 10).Select(_ => Task.Run(async () => await _signInManager.AuthenticateAsync("threadsignin", "threadpw")));
        string?[] results = await Task.WhenAll(tasks);
        Assert.All(results, jwt => Assert.False(string.IsNullOrWhiteSpace(jwt)));
    }

    [Fact(DisplayName = "SignInManager supports multi-session (multiple contexts)"), Trait("Category", "Concurrency")]
    public async Task SignInManager_MultiSession_Support()
    {
        ApplicationUser user = new() { UserName = "multisignin", Email = "multi@example.com" };
        await _userManager.AddUserAsync(user);
        byte[] salt = Guid.NewGuid().ToByteArray();
        string hash = InvokeHashPassword("multipw", salt);
        user.PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}";
        _ = _dbContext.Users.Update(user);
        _ = await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        DesignTimeDbContextFactory factory = new();
        List<ApplicationDbContext> contexts = Enumerable.Range(0, 3).Select(_ => factory.CreateDbContext(Array.Empty<string>())).ToList();
        try
        {
            IEnumerable<Task<string?>> tasks = contexts.Select(ctx => Task.Run(async () =>
            {
                UserManager mgr = new(() => ctx);
                SignInManager signIn = new(mgr, _jwtKey, _jwtIssuer, _jwtAudience);
                return await signIn.AuthenticateAsync("multisignin", "multipw");
            }));
            string?[] results = await Task.WhenAll(tasks);
            Assert.All(results, jwt => Assert.False(string.IsNullOrWhiteSpace(jwt)));
        }
        finally
        {
            foreach (ApplicationDbContext ctx in contexts) ctx.Dispose();
        }
    }

    [Fact(DisplayName = "AuthenticateAsync performance benchmark"), Trait("Category", "Benchmark")]
    public async Task AuthenticateAsync_Performance_Benchmark()
    {
        ApplicationUser user = new() { UserName = "benchuser", Email = "bench@example.com" };
        await _userManager.AddUserAsync(user);
        byte[] salt = Guid.NewGuid().ToByteArray();
        string hash = InvokeHashPassword("benchpw", salt);
        user.PasswordHash = $"{Convert.ToBase64String(salt)}:{hash}";
        _ = _dbContext.Users.Update(user);
        _ = await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Stopwatch sw = Stopwatch.StartNew();
        for (int i = 0; i < 50; i++)
        {
            string? jwt = await _signInManager.AuthenticateAsync("benchuser", "benchpw");
            Assert.False(string.IsNullOrWhiteSpace(jwt));
        }
        sw.Stop();
        Assert.InRange(sw.ElapsedMilliseconds, 0, 3000); // Should be fast enough for 50 JWTs
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

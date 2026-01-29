using Npgsql;

namespace TirsvadCLI.Portfolio.Core.Tests;

public static class ConnectionStringHelper
{
    public static string GetDefaultConnectionString(string databaseName)
    {
        string? connStr = Environment.GetEnvironmentVariable("TCLI_PORTFOLIO_CONNECTIONSTRINGS__DEFAULTCONNECTION");
        if (string.IsNullOrWhiteSpace(connStr))
        {
            connStr = "Host=localhost;Port=5434;Database=portfolio;Username=postgres;Password=postgres";
        }
        NpgsqlConnectionStringBuilder builder = new(connStr)
        {
            Database = databaseName
        };
        return builder.ConnectionString;
    }
}

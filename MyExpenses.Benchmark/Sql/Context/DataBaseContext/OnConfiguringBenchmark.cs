using System.Text;
using BenchmarkDotNet.Attributes;
using Microsoft.Data.Sqlite;

namespace MyExpenses.Benchmark.Sql.Context.DataBaseContext;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class OnConfiguringBenchmark
{
    private const string DataSource = "mydatabase.db";
    private const bool IsReadOnly = true;
    private const bool Pooling = false;

    [Benchmark]
    public string UsingSqliteConnectionStringBuilder()
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = DataSource,
            Pooling = false,
            Mode = IsReadOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite
        };
        return connectionStringBuilder.ConnectionString;
    }

    [Benchmark]
    public string UsingManualStringConstruction()
    {
        return BuildConnectionString(DataSource, IsReadOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite, Pooling);
    }

    [Benchmark]
    public string UsingStringBuilderConstruction()
    {
        return BuildConnectionStringWithStringBuilder(DataSource, IsReadOnly ? SqliteOpenMode.ReadOnly : SqliteOpenMode.ReadWrite, Pooling);
    }

    private static string BuildConnectionString(string dataSource,
        SqliteOpenMode mode = SqliteOpenMode.ReadWrite,
        bool pooling = false,
        SqliteCacheMode cache = SqliteCacheMode.Default,
        string? password = null,
        bool? foreignKeys = null,
        bool recursiveTriggers = false,
        bool browsableConnectionString = false,
        int defaultTimeout = 0)
    {
        // if (string.IsNullOrWhiteSpace(dataSource))
        //     throw new ArgumentException(@"DataSource cannot be null or empty", nameof(dataSource));

        var parts = new List<string>
        {
            $"Data Source=\"{dataSource}\"",
            $"Mode={mode}",
            $"Pooling={pooling.ToString()}"
        };

        if (cache is not SqliteCacheMode.Default) parts.Add($"Cache={cache}");
        if (!string.IsNullOrEmpty(password)) parts.Add($"Password={password}");
        if (foreignKeys.HasValue) parts.Add($"Foreign Keys={foreignKeys.Value}");
        if (recursiveTriggers) parts.Add($"Recursive Triggers={recursiveTriggers.ToString()}");
        if (browsableConnectionString) parts.Add($"Browsable Connection String={browsableConnectionString.ToString()}");
        if (defaultTimeout is not 0) parts.Add($"Default Timeout={defaultTimeout}");

        return string.Join(";", parts);
    }

    private static string BuildConnectionStringWithStringBuilder(string dataSource,
        SqliteOpenMode mode = SqliteOpenMode.ReadWrite,
        bool pooling = false,
        SqliteCacheMode cache = SqliteCacheMode.Default,
        string? password = null,
        bool? foreignKeys = null,
        bool recursiveTriggers = false,
        bool browsableConnectionString = false,
        int defaultTimeout = 0)
    {
        // if (string.IsNullOrWhiteSpace(dataSource))
        //     throw new ArgumentException(@"DataSource cannot be null or empty", nameof(dataSource));

        var builder = new StringBuilder();

        builder.Append($"Data Source=\"{dataSource}\";")
            .Append($"Mode={mode};")
            .Append($"Pooling={pooling};");

        if (cache is not SqliteCacheMode.Default) builder.Append($"Cache={cache};");
        if (!string.IsNullOrEmpty(password)) builder.Append($"Password={password};");
        if (foreignKeys.HasValue) builder.Append($"Foreign Keys={foreignKeys.Value};");
        if (recursiveTriggers) builder.Append($"Recursive Triggers={recursiveTriggers};");
        if (browsableConnectionString) builder.Append($"Browsable Connection String={browsableConnectionString};");
        if (defaultTimeout is not 0) builder.Append($"Default Timeout={defaultTimeout};");
        if (builder.Length > 0 && builder[^1] is ';') builder.Remove(builder.Length - 1, 1);

        return builder.ToString();
    }
}
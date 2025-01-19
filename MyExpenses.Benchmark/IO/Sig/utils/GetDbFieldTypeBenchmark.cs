using BenchmarkDotNet.Attributes;
using NetTopologySuite.IO.Esri.Dbf;

namespace MyExpenses.Benchmark.IO.Sig.utils;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class GetDbFieldTypeBenchmark
{
    private static readonly Dictionary<DbfType, string> FieldTypeMap = new()
    {
        { DbfType.Numeric, "long" },
        { DbfType.Float, "double" },
        { DbfType.Character, "string" },
        { DbfType.Date, "DateTime" },
        { DbfType.Logical, "int" }
    };

    private static readonly DbfType[] TestDbfTypes =
        [DbfType.Numeric, DbfType.Float, DbfType.Character, DbfType.Date, DbfType.Logical];


    private static string GetDbFieldTypeFromDictionary(DbfType dbfType)
    {
        return FieldTypeMap[dbfType];
    }

    private static string GetDbFieldTypeFromSwitch(DbfType dbfType)
    {
        return dbfType switch
        {
            DbfType.Numeric => "long",
            DbfType.Float => "double",
            DbfType.Character => "string",
            DbfType.Date => "DateTime",
            DbfType.Logical => "int",
            _ => throw new ArgumentOutOfRangeException(nameof(dbfType), dbfType, @"Unsupported DbfType")
        };
    }

    [Benchmark]
    public void BenchmarkDictionary()
    {
        foreach (var dbfType in TestDbfTypes)
        {
            _ = GetDbFieldTypeFromDictionary(dbfType);
        }
    }

    [Benchmark]
    public void BenchmarkSwitch()
    {
        foreach (var dbfType in TestDbfTypes)
        {
            _ = GetDbFieldTypeFromSwitch(dbfType);
        }
    }
}
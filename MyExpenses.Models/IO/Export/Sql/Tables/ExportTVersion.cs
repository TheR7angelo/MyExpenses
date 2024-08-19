using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTVersion
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("version")]
    [DisplayName("version")]
    public string? VersionStr { get; set; }

    [Ignore]
    public Version? Version
        => VersionStr is not null ? new Version(VersionStr) : null;
}
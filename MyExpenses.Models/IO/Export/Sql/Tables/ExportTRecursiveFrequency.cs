using System.ComponentModel;
using CsvHelper.Configuration.Attributes;
using MyExpenses.Models.Sql.Bases.Enums;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTRecursiveFrequency
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Ignore]
    public ERecursiveFrequency ERecursiveFrequency
        => (ERecursiveFrequency)Id;

    [Name("frequency")]
    [DisplayName("frequency")]
    public string? Frequency { get; set; }

    [Name("description")]
    [DisplayName("description")]
    public string? Description { get; set; }
}
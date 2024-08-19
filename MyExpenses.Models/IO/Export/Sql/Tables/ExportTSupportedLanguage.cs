using System.ComponentModel;
using CsvHelper.Configuration.Attributes;

namespace MyExpenses.Models.IO.Export.Sql.Tables;

public class ExportTSupportedLanguage
{
    [Name("id")]
    [DisplayName("id")]
    public int Id { get; set; }

    [Name("code")]
    [DisplayName("code")]
    public string Code { get; set; }

    [Name("native_name")]
    [DisplayName("native_name")]
    public string NativeName { get; set; }

    [Name("english_name")]
    [DisplayName("english_name")]
    public string EnglishName { get; set; }

    [Name("default_language")]
    [DisplayName("default_language")]
    public bool? DefaultLanguage { get; set; }

    [Name("date_added")]
    [DisplayName("date_added")]
    public DateTime? DateAdded { get; set; }
}
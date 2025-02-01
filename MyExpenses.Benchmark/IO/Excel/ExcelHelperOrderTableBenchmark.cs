using BenchmarkDotNet.Attributes;
using OfficeOpenXml;
using OfficeOpenXml.Sorting;
using OfficeOpenXml.Table;

namespace MyExpenses.Benchmark.IO.Excel;

[MemoryDiagnoser]
[ThreadingDiagnoser]
[AllStatisticsColumn]
[HtmlExporter]
[MarkdownExporter]
[RankColumn]
public class ExcelHelperOrderTableBenchmark
{
    private ExcelTable _excelTable = null!;
    private readonly Type _onType = typeof(TestRow);
    private const string OnPropertyName = nameof(TestRow.Id);
    private const eSortOrder SortOrder = eSortOrder.Ascending;

    [GlobalSetup]
    public void Setup()
    {
        var package = new ExcelPackage();
        var sheet = package.Workbook.Worksheets.Add("Sheet1");

        sheet.Cells[1, 1].Value = "ID";
        sheet.Cells[1, 2].Value = "Name";
        for (var i = 2; i <= 1001; i++)
        {
            sheet.Cells[i, 1].Value = i - 1;
            sheet.Cells[i, 2].Value = "Name_" + (i - 1);
        }

        var range = sheet.Cells[1, 1, 1001, 2];
        _excelTable = sheet.Tables.Add(range, "Table1");
    }

    [Benchmark]
    public void MethodWithTableSortOptions()
    {
        var index = Array.IndexOf(_onType.GetProperties(), _onType.GetProperty(OnPropertyName));
        var sorting = new TableSortOptions(_excelTable);
        sorting.SortBy.Column(index, SortOrder);
        _excelTable.Sort(sorting);
    }

    [Benchmark]
    public void MethodWithLambda()
    {
        var index = Array.IndexOf(_onType.GetProperties(), _onType.GetProperty(OnPropertyName));
        _excelTable.Sort(s => s.SortBy.Column(index, SortOrder));
    }

    private class TestRow
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

}

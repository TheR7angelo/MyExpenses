using System.Reflection;
using MyExpenses.IO.Excel;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using OfficeOpenXml;

namespace MyExpenses.IO.Test;

public class Test
{
    [Fact]
    private void SecondTest()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
        using var context = new DataBaseContext(dbFile);

        using var package = new ExcelPackage();
        var workbook = package.Workbook;

        var exportVRecursiveFrequency = context.ExportVRecursiveFrequencies.AsEnumerable();
        var exportVAccountType = context.ExportVAccountTypes.AsEnumerable();

        _ = workbook.SetTableCollection(exportVRecursiveFrequency, context);
        var exportVAccountTypeRange = workbook.SetTableCollection(exportVAccountType, context);

        const string filePath = "Test.xlsx";
        package.SaveAs(filePath);
    }


    // [Fact]
    // private void ToExcel()
    // {
    //     var executablePath = Assembly.GetExecutingAssembly().Location;
    //     var path = executablePath.GetParentDirectory(6);
    //     var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
    //     using var context = new DataBaseContext(dbFile);
    //
    //     using var package = new ExcelPackage();
    //     var workbook = package.Workbook;
    //     var worksheet = workbook.Worksheets.Add("Sheet1");
    //
    //     var records = from ta in context.TAccounts
    //                   join tat in context.TAccountTypes on ta.AccountTypeFk equals tat.Id
    //                   join tc in context.TCurrencies on ta.CurrencyFk equals tc.Id
    //                   select new
    //                   {
    //                       ta.Id,
    //                       AccountName = ta.Name,
    //                       AccountType = tat.Name,
    //                       tc.Symbol,
    //                       ta.Active,
    //                       ta.DateAdded
    //                   };
    //     var results = records.AsEnumerable();
    //     var range = worksheet.Cells["A1"].LoadFromCollection(results, true);
    //     SetExcelTableStyle(worksheet, range, "test");
    //
    //     const int accountTypeColumnIndex = 3;
    //
    //     var options = context.TAccountTypes.Select(ct => ct.Name).Distinct().AsEnumerable();
    //     var concatenatedOptions = string.Join(',', options);
    //
    //     var validation = worksheet.DataValidations.AddListValidation(worksheet.Cells[2, accountTypeColumnIndex, worksheet.Dimension.End.Row, accountTypeColumnIndex].Address);
    //     validation.Formula.Values.Add(concatenatedOptions);
    //
    //     const string filePath = "test.xlsx";
    //     package.SaveAs(filePath);
    // }
}
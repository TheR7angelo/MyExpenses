using System.Reflection;
using MyExpenses.IO.Excel;
using MyExpenses.Models.Sql.Bases.Views.Exports;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation.Contracts;
using OfficeOpenXml.Table;

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

        var worksheet = workbook.Worksheets.Add("param");
        worksheet.Cells["A1"].Value = "Boolean value";
        worksheet.Cells["A2"].Value = false;
        worksheet.Cells["A3"].Value = true;
        var range = worksheet.Cells["A1:A3"];
        worksheet.SetExcelTableStyle(range, "Boolean_value");
        worksheet.Hidden = eWorkSheetHidden.VeryHidden;

        // var exportVRecursiveFrequency = context.ExportVRecursiveFrequencies.AsEnumerable();
        var exportVAccountType = context.ExportVAccountTypes.AsEnumerable();
        var exportVCurrency = context.ExportVCurrencies.AsEnumerable();
        var exportVAccount = context.ExportVAccounts.AsEnumerable();

        var exportVAccountTypeTable = workbook.AddTableCollection(exportVAccountType, context);
        var exportVCurrencyTable = workbook.AddTableCollection(exportVCurrency, context);
        var exportVAccountRange = workbook.AddTableCollection(exportVAccount, context);

        exportVAccountRange.AddListValidation(exportVAccountTypeTable, typeof(ExportVAccount), nameof(ExportVAccount.AccountType), nameof(ExportVAccountType.Name));
        exportVAccountRange.AddListValidation(exportVCurrencyTable, typeof(ExportVAccount), nameof(ExportVAccount.Currency), nameof(ExportVCurrency.Symbol));

        // _ = workbook.SetTableCollection(exportVRecursiveFrequency, context);
        const string filePath = "Test.xlsx";
        package.SaveAs(filePath);
    }
}
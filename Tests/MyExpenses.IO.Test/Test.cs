using System.Reflection;
using MyExpenses.IO.Excel;
using MyExpenses.Models.Sql.Bases.Views.Exports;
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

        var booleanTable = workbook.AddBooleanTable();

        var exportVAccountType = context.ExportVAccountTypes.AsEnumerable();
        var exportVCurrency = context.ExportVCurrencies.AsEnumerable();
        var exportVAccount = context.ExportVAccounts.AsEnumerable();

        var exportVAccountTypeTable = workbook.AddTableCollection(exportVAccountType, context);
        var exportVCurrencyTable = workbook.AddTableCollection(exportVCurrency, context);
        var exportVAccountTable = workbook.AddTableCollection(exportVAccount, context);

        exportVAccountTable.AddListValidation(exportVAccountTypeTable, typeof(ExportVAccount), nameof(ExportVAccount.AccountType), nameof(ExportVAccountType.Name));
        exportVAccountTable.AddListValidation(exportVCurrencyTable, typeof(ExportVAccount), nameof(ExportVAccount.Currency), nameof(ExportVCurrency.Symbol));
        exportVAccountTable.AddListValidationTrueFalse(booleanTable, typeof(ExportVAccount), nameof(ExportVAccount.Active));

        var exportVColor = context.ExportVColors.AsEnumerable();
        var exportVCategoryType = context.ExportVCategoryTypes.AsEnumerable();

        var exportVColorTable = workbook.AddTableCollection(exportVColor, context);
        var exportVCategoryTypeTable = workbook.AddTableCollection(exportVCategoryType, context);

        exportVCategoryTypeTable.AddListValidation(exportVColorTable, typeof(ExportVCategoryType), nameof(ExportVCategoryType.ColorName), nameof(ExportVColor.Name));

        var exportVBankTransfert = context.ExportVBankTransfers.AsEnumerable();

        var exportVBankTransferTypeTable = workbook.AddTableCollection(exportVBankTransfert, context);

        exportVBankTransferTypeTable.AddListValidation(exportVAccountTable, typeof(ExportVBankTransfer), nameof(ExportVBankTransfer.FromAccountName), nameof(ExportVAccount.Name));
        exportVBankTransferTypeTable.AddListValidation(exportVAccountTable, typeof(ExportVBankTransfer), nameof(ExportVBankTransfer.ToAccountName), nameof(ExportVAccount.Name));

        var exportVModePayment = context.ExportVModePayments.AsEnumerable();
        var exportVPlace = context.ExportVPlaces.AsEnumerable();
        var exportVRecursiveFrequency = context.ExportVRecursiveFrequencies.AsEnumerable();
        var exportVRecursiveExpense = context.ExportVRecursiveExpenses.AsEnumerable();

        var exportVModePaymentTable = workbook.AddTableCollection(exportVModePayment, context);
        var exportVPlaceTable = workbook.AddTableCollection(exportVPlace, context);
        var exportVRecursiveFrequencyTable = workbook.AddTableCollection(exportVRecursiveFrequency, context);
        var exportVRecursiveExpenseTable = workbook.AddTableCollection(exportVRecursiveExpense, context);

        exportVRecursiveExpenseTable.AddListValidation(exportVAccountTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.AccountName), nameof(ExportVAccount.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVCategoryTypeTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.CategoryType), nameof(ExportVCategoryType.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVModePaymentTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.ModePayment), nameof(ExportVModePayment.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVPlaceTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.PlaceName), nameof(ExportVPlace.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVRecursiveFrequencyTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.Frequency), nameof(ExportVRecursiveFrequency.Frequency));

        const string filePath = "Test.xlsx";
        package.SaveAs(filePath);
    }
}
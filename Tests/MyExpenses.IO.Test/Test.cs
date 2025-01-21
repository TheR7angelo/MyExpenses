using System.Reflection;
using MyExpenses.IO.Csv;
using MyExpenses.IO.Excel;
using MyExpenses.Models.IO.Excel;
using MyExpenses.Models.Sql.Bases.Views.Exports;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Sql.Context;
using OfficeOpenXml;

namespace MyExpenses.IO.Test;

public class Test
{
    [Fact]
    private void ExcelTest()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
        using var context = new DataBaseContext(dbFile);

        using var package = new ExcelPackage();
        var workbook = package.Workbook;

        var exportVAccountType = context.ExportVAccountTypes.AsEnumerable();
        var exportVCurrency = context.ExportVCurrencies.AsEnumerable();
        var exportVAccount = context.ExportVAccounts.AsEnumerable();
        var exportVColor = context.ExportVColors.AsEnumerable();
        var exportVCategoryType = context.ExportVCategoryTypes.AsEnumerable();
        var exportVBankTransfer = context.ExportVBankTransfers.AsEnumerable();
        var exportVModePayment = context.ExportVModePayments.AsEnumerable();
        var exportVPlace = context.ExportVPlaces.AsEnumerable();
        var exportVRecursiveFrequency = context.ExportVRecursiveFrequencies.AsEnumerable();
        var exportVRecursiveExpense = context.ExportVRecursiveExpenses.AsEnumerable();
        var exportVHistory = context.ExportVHistories.AsEnumerable();

        var exportVHistoryTable = workbook.AddTableCollection(exportVHistory, context, ETableLevel.Level3);
        var exportVBankTransferTypeTable = workbook.AddTableCollection(exportVBankTransfer, context, ETableLevel.Level3);
        var exportVRecursiveExpenseTable = workbook.AddTableCollection(exportVRecursiveExpense, context, ETableLevel.Level3);

        var exportVAccountTable = workbook.AddTableCollection(exportVAccount, context, ETableLevel.Level2);
        var exportVCategoryTypeTable = workbook.AddTableCollection(exportVCategoryType, context, ETableLevel.Level2);

        var exportVAccountTypeTable = workbook.AddTableCollection(exportVAccountType, context, ETableLevel.Level1);
        var exportVCurrencyTable = workbook.AddTableCollection(exportVCurrency, context, ETableLevel.Level1);
        var exportVColorTable = workbook.AddTableCollection(exportVColor, context, ETableLevel.Level1);
        var exportVModePaymentTable = workbook.AddTableCollection(exportVModePayment, context, ETableLevel.Level1);
        var exportVPlaceTable = workbook.AddTableCollection(exportVPlace, context, ETableLevel.Level1);
        var exportVRecursiveFrequencyTable = workbook.AddTableCollection(exportVRecursiveFrequency, context, ETableLevel.Level1);

        var booleanTable = workbook.AddBooleanTable();

        exportVAccountTable.AddListValidation(exportVAccountTypeTable, typeof(ExportVAccount), nameof(ExportVAccount.AccountType), nameof(ExportVAccountType.Name));
        exportVAccountTable.AddListValidation(exportVCurrencyTable, typeof(ExportVAccount), nameof(ExportVAccount.Currency), nameof(ExportVCurrency.Symbol));
        exportVAccountTable.AddListValidationTrueFalse(booleanTable, typeof(ExportVAccount), nameof(ExportVAccount.Active));

        exportVCategoryTypeTable.AddListValidation(exportVColorTable, typeof(ExportVCategoryType), nameof(ExportVCategoryType.ColorName), nameof(ExportVColor.Name));

        exportVBankTransferTypeTable.AddListValidation(exportVAccountTable, typeof(ExportVBankTransfer), nameof(ExportVBankTransfer.FromAccountName), nameof(ExportVAccount.Name));
        exportVBankTransferTypeTable.AddListValidation(exportVAccountTable, typeof(ExportVBankTransfer), nameof(ExportVBankTransfer.ToAccountName), nameof(ExportVAccount.Name));

        exportVRecursiveExpenseTable.AddListValidation(exportVAccountTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.AccountName), nameof(ExportVAccount.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVCategoryTypeTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.CategoryType), nameof(ExportVCategoryType.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVModePaymentTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.ModePayment), nameof(ExportVModePayment.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVPlaceTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.PlaceName), nameof(ExportVPlace.Name));
        exportVRecursiveExpenseTable.AddListValidation(exportVRecursiveFrequencyTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.Frequency), nameof(ExportVRecursiveFrequency.Frequency));
        exportVRecursiveExpenseTable.AddListValidationTrueFalse(booleanTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.IsActive));
        exportVRecursiveExpenseTable.AddListValidationTrueFalse(booleanTable, typeof(ExportVRecursiveExpense), nameof(ExportVRecursiveExpense.ForceDeactivate));

        exportVHistoryTable.AddListValidation(exportVAccountTable, typeof(ExportVHistory), nameof(ExportVHistory.AccountName), nameof(ExportVAccount.Name));
        exportVHistoryTable.AddListValidation(exportVCategoryTypeTable, typeof(ExportVHistory), nameof(ExportVHistory.CategoryType), nameof(ExportVCategoryType.Name));
        exportVHistoryTable.AddListValidation(exportVModePaymentTable, typeof(ExportVHistory), nameof(ExportVHistory.ModePayment), nameof(ExportVModePayment.Name));
        exportVHistoryTable.AddListValidation(exportVPlaceTable, typeof(ExportVHistory), nameof(ExportVHistory.Place), nameof(ExportVPlace.Name));
        exportVHistoryTable.AddListValidationTrueFalse(booleanTable, typeof(ExportVHistory), nameof(ExportVHistory.IsPointed));

        exportVHistoryTable.OrderTable(typeof(ExportVHistory), nameof(ExportVHistory.Date), eSortOrder.Descending);

        var filePath = Path.GetFullPath("Test.xlsx");
        package.SaveAs(filePath);

        filePath.StartFile();
    }

    [Fact]
    private void CsvTest()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Combine(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");
        using var context = new DataBaseContext(dbFile);

        var exportVAccountTypeName = context.GetTableName(typeof(ExportVAccountType));
        var exportVAccountType = context.ExportVAccountTypes.AsEnumerable();
        exportVAccountType.WriteCsv(exportVAccountTypeName!);

        var exportVCurrencyName = context.GetTableName(typeof(ExportVCurrency));
        var exportVCurrency = context.ExportVCurrencies.AsEnumerable();
        exportVCurrency.WriteCsv(exportVCurrencyName!);

        var exportVAccountName = context.GetTableName(typeof(ExportVAccount));
        var exportVAccount = context.ExportVAccounts.AsEnumerable();
        exportVAccount.WriteCsv(exportVAccountName!);

        var exportVColorName = context.GetTableName(typeof(ExportVColor));
        var exportVColor = context.ExportVColors.AsEnumerable();
        exportVColor.WriteCsv(exportVColorName!);

        var exportVCategoryTypeName = context.GetTableName(typeof(ExportVCategoryType));
        var exportVCategoryType = context.ExportVCategoryTypes.AsEnumerable();
        exportVCategoryType.WriteCsv(exportVCategoryTypeName!);

        var exportVBankTransferName = context.GetTableName(typeof(ExportVBankTransfer));
        var exportVBankTransfer = context.ExportVBankTransfers.AsEnumerable();
        exportVBankTransfer.WriteCsv(exportVBankTransferName!);

        var exportVModePaymentName = context.GetTableName(typeof(ExportVModePayment));
        var exportVModePayment = context.ExportVModePayments.AsEnumerable();
        exportVModePayment.WriteCsv(exportVModePaymentName!);

        var exportVPlaceName = context.GetTableName(typeof(ExportVPlace));
        var exportVPlace = context.ExportVPlaces.AsEnumerable();
        exportVPlace.WriteCsv(exportVPlaceName!);

        var exportVRecursiveFrequencyName = context.GetTableName(typeof(ExportVRecursiveFrequency));
        var exportVRecursiveFrequency = context.ExportVRecursiveFrequencies.AsEnumerable();
        exportVRecursiveFrequency.WriteCsv(exportVRecursiveFrequencyName!);

        var exportVRecursiveExpenseName = context.GetTableName(typeof(ExportVRecursiveExpense));
        var exportVRecursiveExpense = context.ExportVRecursiveExpenses.AsEnumerable();
        exportVRecursiveExpense.WriteCsv(exportVRecursiveExpenseName!);

        var exportVHistoryName = context.GetTableName(typeof(ExportVHistory));
        var exportVHistory = context.ExportVHistories.AsEnumerable();
        exportVHistory.WriteCsv(exportVHistoryName!);
    }
}
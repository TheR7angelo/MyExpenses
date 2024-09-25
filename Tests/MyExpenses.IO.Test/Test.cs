using System.Drawing;
using System.Reflection;
using MyExpenses.IO.Excel;
using MyExpenses.Models.Sql.Bases.Views.Exports;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using OfficeOpenXml;
using OfficeOpenXml.Sorting;
using OfficeOpenXml.Table;
using Xunit.Abstractions;

namespace MyExpenses.IO.Test;

public class Test
{
    private readonly Color _colorLevel1 = Color.Aqua;
    private readonly Color _colorLevel2 = Color.Blue;
    private readonly Color _colorLevel3 = Color.Green;

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

        var exportVHistoryTable = workbook.AddTableCollection(exportVHistory, context, _colorLevel3);
        var exportVBankTransferTypeTable = workbook.AddTableCollection(exportVBankTransfer, context, _colorLevel3);
        var exportVRecursiveExpenseTable = workbook.AddTableCollection(exportVRecursiveExpense, context, _colorLevel3);

        var exportVAccountTable = workbook.AddTableCollection(exportVAccount, context, _colorLevel2);
        var exportVCategoryTypeTable = workbook.AddTableCollection(exportVCategoryType, context, _colorLevel2);

        var exportVAccountTypeTable = workbook.AddTableCollection(exportVAccountType, context, _colorLevel1);
        var exportVCurrencyTable = workbook.AddTableCollection(exportVCurrency, context, _colorLevel1);
        var exportVColorTable = workbook.AddTableCollection(exportVColor, context, _colorLevel1);
        var exportVModePaymentTable = workbook.AddTableCollection(exportVModePayment, context, _colorLevel1);
        var exportVPlaceTable = workbook.AddTableCollection(exportVPlace, context, _colorLevel1);
        var exportVRecursiveFrequencyTable = workbook.AddTableCollection(exportVRecursiveFrequency, context, _colorLevel1);

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
        exportVHistoryTable.AddListValidationTrueFalse(booleanTable, typeof(ExportVHistory), nameof(ExportVHistory.Pointed));

        exportVHistoryTable.OrderTable(typeof(ExportVHistory), nameof(ExportVHistory.Date), eSortOrder.Descending);

        const string filePath = "Test.xlsx";
        package.SaveAs(filePath);
    }
}
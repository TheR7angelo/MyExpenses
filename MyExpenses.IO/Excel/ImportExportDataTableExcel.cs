using System.Reflection;
using MyExpenses.Models.IO.Excel;
using MyExpenses.Models.Sql.Bases.Views.Exports;
using MyExpenses.Sql.Context;
using OfficeOpenXml;
using Serilog;

namespace MyExpenses.IO.Excel;

public static class ImportExportDataTableExcel
{
    static ImportExportDataTableExcel()
    {
        var assembly = Assembly.GetEntryAssembly();
        var attributes = assembly?.GetCustomAttributes().ToList();
        var companyAttribute = attributes?.OfType<AssemblyCompanyAttribute>().FirstOrDefault();

        ExcelPackage.License.SetNonCommercialPersonal(companyAttribute?.Company);
    }

    public static bool ToExcelWorksheet(this DataBaseContextOld contextOld, string filePath)
    {
        filePath = Path.ChangeExtension(filePath, ".xlsx");

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The allocation of ExcelPackage is necessary for creating and managing the Excel workbook.
        // It is properly used within a 'using' statement, ensuring resources are disposed of correctly after use.
        // This allocation is essential for the operation and does not have a significant impact on performance.
        using var package = new ExcelPackage();
        var workbook = package.Workbook;

        try
        {
            var exportVAccountType = contextOld.ExportVAccountTypes.AsEnumerable();
            var exportVCurrency = contextOld.ExportVCurrencies.AsEnumerable();
            var exportVAccount = contextOld.ExportVAccounts.AsEnumerable();
            var exportVColor = contextOld.ExportVColors.AsEnumerable();
            var exportVCategoryType = contextOld.ExportVCategoryTypes.AsEnumerable();
            var exportVBankTransfer = contextOld.ExportVBankTransfers.AsEnumerable();
            var exportVModePayment = contextOld.ExportVModePayments.AsEnumerable();
            var exportVPlace = contextOld.ExportVPlaces.AsEnumerable();
            var exportVRecursiveFrequency = contextOld.ExportVRecursiveFrequencies.AsEnumerable();
            var exportVRecursiveExpense = contextOld.ExportVRecursiveExpenses.AsEnumerable();
            var exportVHistory = contextOld.ExportVHistories.AsEnumerable();

            var exportVHistoryTable = workbook.AddTableCollection(exportVHistory, contextOld, ETableLevel.Level3);
            var exportVBankTransferTypeTable = workbook.AddTableCollection(exportVBankTransfer, contextOld, ETableLevel.Level3);
            var exportVRecursiveExpenseTable = workbook.AddTableCollection(exportVRecursiveExpense, contextOld, ETableLevel.Level3);

            var exportVAccountTable = workbook.AddTableCollection(exportVAccount, contextOld, ETableLevel.Level2);
            var exportVCategoryTypeTable = workbook.AddTableCollection(exportVCategoryType, contextOld, ETableLevel.Level2);

            var exportVAccountTypeTable = workbook.AddTableCollection(exportVAccountType, contextOld, ETableLevel.Level1);
            var exportVCurrencyTable = workbook.AddTableCollection(exportVCurrency, contextOld, ETableLevel.Level1);
            var exportVColorTable = workbook.AddTableCollection(exportVColor, contextOld, ETableLevel.Level1);
            var exportVModePaymentTable = workbook.AddTableCollection(exportVModePayment, contextOld, ETableLevel.Level1);
            var exportVPlaceTable = workbook.AddTableCollection(exportVPlace, contextOld, ETableLevel.Level1);
            var exportVRecursiveFrequencyTable = workbook.AddTableCollection(exportVRecursiveFrequency, contextOld, ETableLevel.Level1);

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

            package.SaveAs(filePath);
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while saving Excel file");
            return false;
        }
    }
}
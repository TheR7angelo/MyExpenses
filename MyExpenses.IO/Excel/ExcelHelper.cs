using System.Reflection;
using MyExpenses.Sql.Context;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace MyExpenses.IO.Excel;

public static class ExcelHelper
{
    /// <summary>
    /// Adds list validation of boolean values ("True" or "False") to a specified column in an Excel table.
    /// </summary>
    /// <param name="onExcelTable">The Excel table on which the validation will be applied.</param>
    /// <param name="fromExcelTable">The Excel table providing the list of boolean values for validation.</param>
    /// <param name="onType">The Type of the object representing the structure of the target Excel table.</param>
    /// <param name="onPropertyName">The name of the property in the target Type that corresponds to the column to be validated.</param>
    public static void AddListValidationTrueFalse(this ExcelTable onExcelTable, ExcelTable fromExcelTable, Type onType,
        string onPropertyName)
    {
        var columnHeader = fromExcelTable.Columns[0].Name;

        var index = Array.IndexOf(onType.GetProperties(), onType.GetProperty(onPropertyName)) + 1;
        var validationPlage = onExcelTable.Range.Worksheet.Cells[2, index, onExcelTable.Range.End.Row, index];
        var validation = onExcelTable.Range.Worksheet.DataValidations.AddListValidation(validationPlage.Address);
        validation.Formula.ExcelFormula = $"=INDIRECT(\"{fromExcelTable.Name}[{columnHeader}]\")";
    }

    /// <summary>
    /// Adds list validation to a specified column in an Excel table based on the values from another Excel table.
    /// </summary>
    /// <param name="onExcelTable">The Excel table on which the validation will be applied.</param>
    /// <param name="fromExcelTable">The Excel table providing the list of valid values for validation.</param>
    /// <param name="onType">The Type of the object representing the structure of the target Excel table.</param>
    /// <param name="onPropertyName">The name of the property in the target Type that corresponds to the column to be validated.</param>
    /// <param name="fromPropertyName">The name of the property in the source Excel table that provides the valid values for validation.</param>
    public static void AddListValidation(this ExcelTable onExcelTable, ExcelTable fromExcelTable, Type onType,
        string onPropertyName, string fromPropertyName)
    {
        var index = Array.IndexOf(onType.GetProperties(), onType.GetProperty(onPropertyName)) + 1;
        var validationPlage = onExcelTable.Range.Worksheet.Cells[2, index, onExcelTable.Range.End.Row, index];
        var validation = onExcelTable.Range.Worksheet.DataValidations.AddListValidation(validationPlage.Address);
        validation.Formula.ExcelFormula = $"= INDIRECT(\"{fromExcelTable.Name}[{fromPropertyName}]\")";
    }

    /// <summary>
    /// Adds a collection of data to a new Excel worksheet and formats it as a table.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="workbook">The Excel workbook to which the table will be added.</param>
    /// <param name="collection">The collection of data to be added to the Excel worksheet.</param>
    /// <param name="context">The database context used to retrieve the table name.</param>
    /// <returns>An instance of <see cref="ExcelTable"/> representing the formatted table in the worksheet.</returns>
    public static ExcelTable AddTableCollection<T>(this ExcelWorkbook workbook, IEnumerable<T> collection,
        DataBaseContext context)
    {
        var type = typeof(T);
        var collectionName = context.GetTableName(type);

        var worksheet = workbook.Worksheets.Add(collectionName);
        var range = worksheet.Cells["A1"].LoadFromCollection(collection, true);

        var excelTable = worksheet.SetExcelTableStyle(range, $"{collectionName}_table");
        worksheet.SetDateStyle(type.GetProperties(), range);
        range.AutoFitColumns();

        return excelTable;
    }

    /// <summary>
    /// Adds a worksheet with a predefined boolean table to the Excel workbook.
    /// </summary>
    /// <param name="workbook">The Excel workbook to which the worksheet and boolean table will be added.</param>
    /// <param name="worksheetName">The name of the worksheet to be created. Defaults to "BooleanSheet".</param>
    /// <returns>The created Excel table containing the boolean values.</returns>
    public static ExcelTable AddBooleanTable(this ExcelWorkbook workbook, string worksheetName = "BooleanSheet")
    {
        var worksheet = workbook.Worksheets.Add(worksheetName);
        worksheet.Cells["A1"].Value = "Boolean value";
        worksheet.Cells["A2"].Value = false;
        worksheet.Cells["A3"].Value = true;
        var range = worksheet.Cells["A1:A3"];

        var tableName = worksheetName.Replace(" ", "_");
        var booleanTable = worksheet.SetExcelTableStyle(range, tableName);
        worksheet.Hidden = eWorkSheetHidden.VeryHidden;
        return booleanTable;
    }

    /// <summary>
    /// Applies date formatting to specified columns in an Excel worksheet.
    /// </summary>
    /// <param name="worksheet">The Excel worksheet where the date formatting will be applied.</param>
    /// <param name="headers">An array of PropertyInfo objects representing the headers of each column.</param>
    /// <param name="excelRangeBase">The range within the worksheet where the data is located.</param>
    public static void SetDateStyle(this ExcelWorksheet worksheet, PropertyInfo[] headers,
        ExcelRangeBase excelRangeBase)
    {
        foreach (var header in headers)
        {
            if (header.PropertyType == typeof(DateTime) || header.PropertyType == typeof(DateTime?))
            {
                var index = Array.IndexOf(headers, header) + 1;
                worksheet.Cells[2, index, excelRangeBase.End.Row, index].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
            }

            if (header.PropertyType == typeof(DateOnly) || header.PropertyType == typeof(DateOnly?))
            {
                var index = Array.IndexOf(headers, header) + 1;
                worksheet.Cells[2, index, excelRangeBase.End.Row, index].Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
            }
        }
    }

    /// <summary>
    /// Adds a new table to the specified Excel worksheet using the provided range, applies the given table style, and names the table.
    /// </summary>
    /// <param name="worksheet">The Excel worksheet where the table will be added.</param>
    /// <param name="excelRangeBase">The range of cells that will be included in the table.</param>
    /// <param name="tableName">The name to be assigned to the created table.</param>
    /// <param name="tableStyles">The style to be applied to the table (default is Medium7).</param>
    /// <returns>The created Excel table with the applied style.</returns>
    public static ExcelTable SetExcelTableStyle(this ExcelWorksheet worksheet, ExcelRangeBase excelRangeBase,
        string tableName, TableStyles tableStyles = TableStyles.Medium7)
    {
        var excelTable = worksheet.Tables.Add(excelRangeBase, tableName);
        excelTable.TableStyle = tableStyles;
        return excelTable;
    }
}
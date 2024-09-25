using System.Drawing;
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
        var validationRange = onExcelTable.Range.Worksheet.Cells[2, index, onExcelTable.Range.End.Row, index];
        var validation = onExcelTable.Range.Worksheet.DataValidations.AddListValidation(validationRange.Address);
        validation.Formula.ExcelFormula = $"=INDIRECT(\"{fromExcelTable.Name}[{fromPropertyName}]\")";
    }

    /// <summary>
    /// Adds a collection of data to an Excel workbook as a table with optional tab color customization.
    /// </summary>
    /// <param name="workbook">The Excel workbook where the table will be added.</param>
    /// <param name="collection">The collection of data to be added as a table.</param>
    /// <param name="context">The database context used to retrieve table name information.</param>
    /// <param name="tabColor">Optional tab color for the worksheet.</param>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <returns>The created Excel table.</returns>
    public static ExcelTable AddTableCollection<T>(this ExcelWorkbook workbook, IEnumerable<T> collection,
        DataBaseContext context, Color? tabColor = null)
    {
        var type = typeof(T);
        var collectionName = context.GetTableName(type);

        var worksheet = workbook.Worksheets.Add(collectionName);
        var range = worksheet.Cells["A1"].LoadFromCollection(collection, true);

        var excelTable = worksheet.SetExcelTableStyle(range, $"{collectionName}_table");
        worksheet.SetDateStyle(type.GetProperties(), range);
        range.AutoFitColumns();

        if (tabColor is not null) worksheet.TabColor = tabColor.Value;

        return excelTable;
    }

    /// <summary>
    /// Adds a new worksheet containing boolean values (true/false) as a table to the specified Excel workbook.
    /// </summary>
    /// <param name="workbook">The ExcelWorkbook object to which the boolean table will be added.</param>
    /// <param name="worksheetName">The name of the worksheet to be created. Defaults to "BooleanSheet".</param>
    /// <param name="columnHeader">The header for the boolean values column. Defaults to "Boolean values".</param>
    /// <returns>Returns an ExcelTable object representing the table of boolean values.</returns>
    public static ExcelTable AddBooleanTable(this ExcelWorkbook workbook, string worksheetName = "BooleanSheet",
        string columnHeader = "Boolean values")
    {
        var worksheet = workbook.Worksheets.Add(worksheetName);
        worksheet.Cells["A1"].Value = columnHeader;
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
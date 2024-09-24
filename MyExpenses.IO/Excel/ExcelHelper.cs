using System.Reflection;
using MyExpenses.Sql.Context;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace MyExpenses.IO.Excel;

public static class ExcelHelper
{
    /// <summary>
    /// Creates a new worksheet in the given Excel workbook, loads a collection of data into it, and applies table formatting and date styles.
    /// </summary>
    /// <param name="workbook">The Excel workbook where the new worksheet will be added.</param>
    /// <param name="collection">The collection of data to be loaded into the worksheet.</param>
    /// <param name="context">Database context used to determine the name of the table.</param>
    /// <param name="tableName">Outputs the name of the created table.</param>
    /// <typeparam name="T">The type of the data in the collection.</typeparam>
    /// <returns>The range of cells that were loaded with the collection.</returns>
    public static ExcelRangeBase AddTableCollection<T>(this ExcelWorkbook workbook, IEnumerable<T> collection,
        DataBaseContext context, out string tableName)
    {
        var type = typeof(T);
        var collectionName = context.GetTableName(type);
        tableName = $"{collectionName}_table";

        var worksheet = workbook.Worksheets.Add(collectionName);
        var range = worksheet.Cells["A1"].LoadFromCollection(collection, true);

        worksheet.SetExcelTableStyle(range, tableName);
        worksheet.SetDateStyle(type.GetProperties(), range);
        range.AutoFitColumns();

        return range;
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
    /// Applies a specified Excel table style to a range within a worksheet and names the table.
    /// </summary>
    /// <param name="worksheet">The Excel worksheet where the table will be created.</param>
    /// <param name="excelRangeBase">The range within the worksheet to be styled as a table.</param>
    /// <param name="tableName">The name to be assigned to the table.</param>
    /// <param name="tableStyles">The style to be applied to the table. Defaults to TableStyles.Medium7.</param>
    /// <returns>Returns the ExcelRangeBase object representing the styled table within the worksheet.</returns>
    public static ExcelRangeBase SetExcelTableStyle(this ExcelWorksheet worksheet, ExcelRangeBase excelRangeBase,
        string tableName, TableStyles tableStyles = TableStyles.Medium7)
    {
        var table = worksheet.Tables.Add(excelRangeBase, tableName);
        table.TableStyle = tableStyles;
        return excelRangeBase;
    }
}
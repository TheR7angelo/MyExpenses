using System.Reflection;
using MyExpenses.Sql.Context;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace MyExpenses.IO.Excel;

public static class ExcelHelper
{
    /// <summary>
    /// Sets a collection of data into an Excel table within a workbook and applies styling.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the collection.</typeparam>
    /// <param name="workbook">The Excel workbook where the table will be created.</param>
    /// <param name="collection">The collection of data to be added to the Excel table.</param>
    /// <param name="context">The database context to retrieve table name information.</param>
    /// <returns>Returns the ExcelRangeBase object representing the created table within the worksheet.</returns>
    public static ExcelRangeBase SetTableCollection<T>(this ExcelWorkbook workbook, IEnumerable<T> collection,
        DataBaseContext context)
    {
        var type = typeof(T);
        var collectionName = context.GetTableName(type);

        var worksheet = workbook.Worksheets.Add(collectionName);
        var range = worksheet.Cells["A1"].LoadFromCollection(collection, true);
        worksheet.SetExcelTableStyle(range, $"{collectionName}_table");
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
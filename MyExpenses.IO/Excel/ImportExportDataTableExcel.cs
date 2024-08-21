using System.ComponentModel;
using System.Reflection;
using MyExpenses.Models.IO.Export;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace MyExpenses.IO.Excel;

public static class ImportExportDataTableExcel
{
    static ImportExportDataTableExcel()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public static bool ToExcelWorksheet(this IEnumerable<ExportRecord> exportRecords, string filePath)
    {
        filePath = Path.ChangeExtension(filePath, ".xlsx");

        using var package = new ExcelPackage();
        using var workbook = package.Workbook;

        foreach (var exportRecord in exportRecords)
        {
            var worksheet = package.Workbook.Worksheets.Add(exportRecord.Name);

            var properties = exportRecord.Records.First()!.GetType().GetProperties();
            var headers = worksheet.SetHeaders(properties);

            worksheet.SetValue(exportRecord, properties, headers);

            worksheet.SetDateStyle(headers);

            var dataRange = worksheet.SetExcelTableStyle(headers, exportRecord);

            dataRange.AutoFitColumns();
        }

        package.SaveAs(filePath);

        return false;
    }

    private static ExcelRange SetExcelTableStyle(this ExcelWorksheet worksheet, List<PropertyInfo> headers, ExportRecord exportRecord)
    {
        var maxRow = worksheet.Dimension.End.Row;
        var dataRange = worksheet.Cells[1, 1, maxRow, headers.Count];
        var table = worksheet.Tables.Add(dataRange, $"{exportRecord.Name}_table");
        table.TableStyle = TableStyles.Medium7;
        return dataRange;
    }

    private static void SetDateStyle(this ExcelWorksheet worksheet, List<PropertyInfo> headers)
    {
        foreach (var propertyInfo in headers)
        {
            if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                var index = headers.IndexOf(propertyInfo) + 1;
                worksheet.Column(index).Style.Numberformat.Format = "yyyy-mm-dd hh:mm:ss";
            }

            if (propertyInfo.PropertyType == typeof(DateOnly) || propertyInfo.PropertyType == typeof(DateOnly?))
            {
                var index = headers.IndexOf(propertyInfo) + 1;
                worksheet.Column(index).Style.Numberformat.Format = "yyyy-mm-dd";
            }
        }
    }

    private static void SetValue(this ExcelWorksheet worksheet, ExportRecord exportRecord, PropertyInfo[] properties,
        List<PropertyInfo> headers)
    {
        var row = 2;
        foreach (var record in exportRecord.Records)
        {
            var column = 1;
            foreach (var propertyInfo in properties)
            {
                if (!headers.Contains(propertyInfo)) continue;

                var value = propertyInfo.GetValue(record);
                worksheet.Cells[row, column].Value = value;

                column++;
            }

            row++;
        }
    }

    private static List<PropertyInfo> SetHeaders(this ExcelWorksheet worksheet, PropertyInfo[] properties)
    {
        var column = 1;
        var headers = new List<PropertyInfo>();
        foreach (var propertyInfo in properties)
        {
            var header = propertyInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
            if (string.IsNullOrEmpty(header)) continue;

            worksheet.Cells[1, column].Value = header;
            headers.Add(propertyInfo);
            column++;
        }

        return headers;
    }
}
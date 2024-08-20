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

    // TODO simplify
    public static bool ToExcelWorksheet(this IEnumerable<ExportRecord> exportRecords, string filePath)
    {
        filePath = Path.ChangeExtension(filePath, ".xlsx");

        using var package = new ExcelPackage();
        using var workbook = package.Workbook;

        foreach (var exportRecord in exportRecords)
        {
            var worksheet = package.Workbook.Worksheets.Add(exportRecord.Name);

            var column = 1;
            var properties = exportRecord.Records.First()!.GetType().GetProperties();
            var headers = new List<PropertyInfo>();
            foreach (var propertyInfo in properties)
            {
                var header = propertyInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
                if (string.IsNullOrEmpty(header)) continue;

                worksheet.Cells[1, column].Value = header;
                headers.Add(propertyInfo);
                column++;
            }

            var row = 2;
            foreach (var record in exportRecord.Records)
            {
                column = 1;
                foreach (var propertyInfo in properties)
                {
                    if (!headers.Contains(propertyInfo)) continue;

                    var value = propertyInfo.GetValue(record);
                    worksheet.Cells[row, column].Value = value;

                    column++;
                }

                row++;
            }

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

            var dataRange = worksheet.Cells[1, 1, row - 1, headers.Count];
            var table = worksheet.Tables.Add(dataRange, $"{exportRecord.Name}_table");
            table.TableStyle = TableStyles.Medium7;

            dataRange.AutoFitColumns();
        }

        package.SaveAs(filePath);

        return false;
    }
}
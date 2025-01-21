using System.Reflection;
using MyExpenses.Models.Sql.Bases.Views.Exports;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Sql.Context;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyExpenses.IO.Test.Pdf;

public class QuestPdfTest
{
    [Fact]
    public void QuestPdfGenerateTest()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var iconFilePath = Path.Combine(path, "MyExpenses.Commons", "Resources", "Assets", "Applications", "Icon.png");

        var iconBytes = File.ReadAllBytes(iconFilePath);
        var  iconFileStream = new FileStream(iconFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        QuestPDF.Settings.License = LicenseType.Community;

        Document
            .Create(container =>
            {
                container.Page(page =>
                {
                    var pageSize = PageSizes.A4;
                    var horizontalPage = new PageSize(pageSize.Height, pageSize.Width);

                    page.Margin(2, Unit.Centimetre); // Adding margins to the document
                    // page.Size(PageSizes.A4);
                    page.Size(horizontalPage);

                    page.Header()
                        .Text("Example with QuestPDF")
                        .FontSize(20)
                        .SemiBold()
                        .AlignCenter();

                    page.Content()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("Introduction")
                                .FontSize(16)
                                .Bold();

                            column.Item()
                                .Text("Welcome to this example document! Here is the introduction section...")
                                .FontSize(12)
                                .LineHeight(1.5f);

                            // Create a page break
                            column.Item().PageBreak();

                            column.Item()
                                .Text("Chapter 1")
                                .FontSize(16)
                                .Bold();

                            column.Item()
                                .Text(
                                    "Here is the content of Chapter 1. This section explains how to use QuestPDF...")
                                .FontSize(12)
                                .LineHeight(1.5f);

                            // Create a page break
                            column.Item().PageBreak();

                            column.Item()
                                .Text("Chapter 2")
                                .FontSize(16)
                                .Bold();

                            column.Item()
                                .Text("This is the content of chapter 2. Now we learn how to manage bookmarks.")
                                .FontSize(12)
                                .LineHeight(1.5f);

                            // // Create a page break
                            // column.Item().PageBreak();
                            //
                            // // Add picture
                            // column.Item()
                            //     .Image(iconFilePath);
                            //
                            // // Add a page break
                            // column.Item().PageBreak();
                            // // Add picture
                            // column.Item().Image(iconBytes);
                            //
                            // // Add a page break
                            // column.Item().PageBreak();
                            // // Add picture
                            // column.Item().Image(iconFileStream);

                            // Add a page break
                            column.Item().PageBreak();

                            //Add table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(5);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn();
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                });

                                var dbFilePath = Path.GetFullPath("Example - en.sqlite");
                                using var context = new DataBaseContext(dbFilePath);

                                var headers = typeof(ExportVHistory).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                    .Select(p => p.Name).ToList();
                                var records = context.ExportVHistories.AsEnumerable();

                                table.Header(header =>
                                {
                                    foreach (var headerText in headers)
                                    {
                                        header.Cell().Border(1.2f).AlignCenter().Text(headerText).FontSize(8)
                                            .Bold().AlignCenter();
                                    }
                                });

                                foreach (var record in records)
                                {
                                    foreach (var headerText in headers)
                                    {
                                        var value = typeof(ExportVHistory).GetProperty(headerText)?.GetValue(record);
                                        table.Cell().Border(1).Text(value?.ToString() ?? string.Empty).AlignCenter().FontSize(8);
                                    }
                                }

                            });
                        });
                });
            })
            .GeneratePdf("QuestPDFTest.pdf");
    }
}
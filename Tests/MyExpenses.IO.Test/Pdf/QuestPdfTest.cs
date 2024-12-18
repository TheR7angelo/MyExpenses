using System.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MyExpenses.IO.Test.Pdf;

public class QuestPdfTest
{
    [Fact]
    public void QuestPdfGenerateTest()
    {
        var iconDirectory = Path.GetFullPath("Pdf");
        var iconFilePath = Path.Join(iconDirectory, "Icon.png");
        var iconBytes = File.ReadAllBytes(iconFilePath);
        var  iconFileStream = new FileStream(iconFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        QuestPDF.Settings.License = LicenseType.Community;

        Document
            .Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(2, Unit.Centimetre); // Adding margins to the document
                    page.Size(PageSizes.A4);

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

                            // Create a page break
                            column.Item().PageBreak();

                            // Add picture
                            column.Item()
                                .Image(iconFilePath);

                            // Add a page break
                            column.Item().PageBreak();
                            // Add picture
                            column.Item().Image(iconBytes);

                            // Add a page break
                            column.Item().PageBreak();
                            // Add picture
                            column.Item().Image(iconFileStream);
                        });
                });
            })
            .GeneratePdf("QuestPDFTest.pdf");
    }
}
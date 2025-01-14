using System.Text;
using MyExpenses.IO.Csv;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.IO.UnitTests.Csv;

public class CsvReaderTest
{
    private static string GetTempFilePath()
    {
        var tempPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();
        var filePath = Path.Join(tempPath, $"{guid}.csv");
        return filePath;
    }

    [Fact]
    public void ReadCsv_ShouldMapRecordsCorrectly_WhenFileIsValid()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var csvContent = "Id;Name;AccountTypeFk;CurrencyFk;Active;DateAdded" + Environment.NewLine +
                         "1;Main Account;101;1;true;2023-10-01" + Environment.NewLine +
                         "2;Savings Account;102;2;false;2023-10-05";

        File.WriteAllText(filePath, csvContent);

        try
        {
            // Act
            var result = filePath.ReadCsv<TAccount>();

            // Assert
            Assert.NotNull(result);
            var records = result.ToList();
            Assert.Equal(2, records.Count);

            Assert.Equal(1, records[0].Id);
            Assert.Equal("Main Account", records[0].Name);
            Assert.True(records[0].Active);

            Assert.Equal(2, records[1].Id);
            Assert.Equal("Savings Account", records[1].Name);
            Assert.False(records[1].Active);
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadCsv_ShouldReturnEmptyCollection_WhenFileIsEmpty()
    {
        // Arrange
        var filePath = GetTempFilePath();
        File.WriteAllText(filePath, string.Empty);

        try
        {
            // Act
            var result = filePath.ReadCsv<TAccount>();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadCsv_ShouldHandleDifferentEncodings()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var csvContent = "Id;Name;AccountTypeFk;CurrencyFk;Active;DateAdded" + Environment.NewLine +
                         "1;Main Account;101;1;true;2023-10-01";

        // Write a CSV file in ISO-8859-1 encoding
        var encoding = Encoding.GetEncoding("ISO-8859-1");
        File.WriteAllText(filePath, csvContent, encoding);

        try
        {
            // Act
            var result = filePath.ReadCsv<TAccount>();

            // Assert
            Assert.NotNull(result);
            var records = result.ToList();
            Assert.Single(records);
            Assert.Equal("Main Account", records[0].Name); // Vérifie que l'encodage est correctement géré
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadCsv_ShouldHandleOptionalColumns_WhenFileHasExtraColumns()
    {
        // Arrange
        var filePath = GetTempFilePath();
        var csvContent = "Id;Name;AccountTypeFk;CurrencyFk;Active;DateAdded;ExtraColumn" + Environment.NewLine +
                         "1;Main Account;101;1;true;2023-10-01;ExtraData" + Environment.NewLine +
                         "2;Savings Account;102;2;false;2023-10-05;ExtraData";

        File.WriteAllText(filePath, csvContent);

        try
        {
            // Act
            var result = filePath.ReadCsv<TAccount>();

            // Assert
            Assert.NotNull(result);
            var records = result.ToList();
            Assert.Equal(2, records.Count);

            Assert.Equal(1, records[0].Id);
            Assert.Equal("Main Account", records[0].Name);

            Assert.Equal(2, records[1].Id);
            Assert.Equal("Savings Account", records[1].Name);
        }
        finally
        {
            // Cleanup
            if (File.Exists(filePath)) File.Delete(filePath);
        }
    }

    [Fact]
    public void ReadCsv_ShouldThrowException_WhenFileDoesNotExist()
    {
        // Arrange
        var invalidFilePath = GetTempFilePath();

        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => invalidFilePath.ReadCsv<TAccount>());
        Assert.Contains("Could not find file", exception.Message);
    }
}
using JetBrains.Annotations;
using MyExpenses.IO.Csv;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.IO.UnitTests.Csv;

[TestSubject(typeof(CsvWriter))]
public class CsvWriterTest
{
    private static TAccount Account1
        => new()
        {
            Id = 1,
            Name = "Main Account",
            AccountTypeFk = 101,
            CurrencyFk = 1,
            Active = true,
            DateAdded = DateTime.Now.AddDays(-5)
        };

    private static TAccount Account2
        => new()
        {
            Id = 2,
            Name = "Savings Account",
            AccountTypeFk = 102,
            CurrencyFk = 2,
            Active = false,
            DateAdded = DateTime.Now
        };

    private static List<TAccount> GetTestRecords()
        => [Account1, Account2];

    private static string GetTempFilePath()
    {
        var tempPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();
        var filePath = Path.Join(tempPath, $"{guid}.csv");
        return filePath;
    }

    [Fact]
    public void WriteCsv_ShouldCreateCsvFile_WhenDataIsValid()
    {
        // Arrange
        var records = GetTestRecords();

        var filePath = GetTempFilePath();

        // Act
        var result = records.WriteCsv(filePath);

        // Assert
        Assert.True(result, "The WriteCsv method should return true if the operation succeeds.");
        Assert.True(File.Exists(filePath), "The CSV file should be created.");

        var csvContent = File.ReadAllText(filePath);
        Assert.Contains(Account1.Name!, csvContent);
        Assert.Contains(Account2.Name!, csvContent);

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void WriteCsv_ShouldHandleEmptyRecords()
    {
        // Arrange
        var records = new List<TAccount>();
        var filePath = Path.GetTempFileName();

        // Act
        var result = records.WriteCsv(filePath);

        // Assert
        Assert.True(result, "The WriteCsv method should return true even if there are no records.");
        Assert.True(File.Exists(filePath), "The CSV file should still be created.");

        var csvContent = File.ReadAllText(filePath);
        Assert.True(string.IsNullOrWhiteSpace(csvContent), "The CSV file should be empty for no records.");

        // Cleanup
        File.Delete(filePath);
    }

    [Fact]
    public void WriteCsv_ShouldReturnFalse_WhenFilePathIsInvalid()
    {
        // Arrange
        var records = GetTestRecords();

        const string invalidFilePath = @"C:\InvalidPath\file.csv";

        // Act
        var result = records.WriteCsv(invalidFilePath);

        // Assert
        Assert.False(result, "The WriteCsv method should return false if the file path is invalid.");
        Assert.False(File.Exists(invalidFilePath), "The CSV file should not be created at an invalid file path.");
    }

    [Fact]
    public void WriteCsv_ShouldHandleFieldsRequiringQuotes()
    {
        // Arrange
        var records = new List<TAccount>
        {
            new TAccount
            {
                Id = 1,
                Name = "Account \"With\" Quotes",
                AccountTypeFk = 101,
                CurrencyFk = 1,
                Active = true,
                DateAdded = DateTime.Now
            }
        };

        var filePath = GetTempFilePath();

        // Act
        var result = records.WriteCsv(filePath);

        // Assert
        Assert.True(result, "The WriteCsv method should return true for valid input.");
        var csvContent = File.ReadAllText(filePath);
        Assert.Contains("\"Account \"\"With\"\" Quotes\"", csvContent, StringComparison.InvariantCulture);

        // Cleanup
        File.Delete(filePath);
    }
}
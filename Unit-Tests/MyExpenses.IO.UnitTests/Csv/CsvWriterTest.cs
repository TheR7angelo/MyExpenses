using JetBrains.Annotations;
using MyExpenses.IO.Csv;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.IO.UnitTests.Csv;

[TestSubject(typeof(CsvWriter))]
public class CsvWriterTest
{
    /// <summary>
    /// Represents a predefined account instance used for testing purposes within the unit tests.
    /// </summary>
    /// <remarks>
    /// This property initializes a <see cref="TAccount"/> instance with specific default values for
    /// testing. These include:
    /// – Id: 1
    /// – Name: "Main Account"
    /// – AccountTypeFk: 101
    /// – CurrencyFk: 1
    /// – Active: true
    /// – DateAdded: 5 days prior to the current date
    /// It is utilized in various tests to validate functionality related to CSV writing and
    /// other account-based operations.
    /// </remarks>
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

    /// <summary>
    /// Represents a secondary account instance used during testing within unit test scenarios.
    /// </summary>
    /// <remarks>
    /// This property initializes a <see cref="TAccount"/> instance with preset values for testing purposes, including:
    /// – Id: 2
    /// – Name: "Savings Account"
    /// – AccountTypeFk: 102
    /// – CurrencyFk: 2
    /// – Active: false
    /// – DateAdded: Current Date and Time
    /// It is primarily utilized in unit tests to verify CSV writing functionality and other account-related features.
    /// </remarks>
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

    /// <summary>
    /// Generates a list of predefined test records consisting of TAccount objects for use in unit tests.
    /// The test records include specific test data simulating different account scenarios.
    /// </summary>
    /// <returns>A list of TAccount objects containing the predefined test data.</returns>
    private static List<TAccount> GetTestRecords()
        => [Account1, Account2];

    private static string GetTempFilePath()
    {
        var tempPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();
        var filePath = Path.Join(tempPath, $"{guid}.csv");
        return filePath;
    }

    /// <summary>
    /// Validates that the WriteCsv method successfully creates a CSV file when given valid input data.
    /// The test verifies the file's existence, ensures the method returns true upon success,
    /// and checks that the content of the CSV file matches the test data provided.
    /// </summary>
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

    /// <summary>
    /// Tests the functionality of the WriteCsv method for handling empty collections of TAccount objects.
    /// Verifies that the method creates an empty CSV file and returns true when there are no records provided.
    /// </summary>
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

    /// <summary>
    /// Validates that the WriteCsv method returns false when an invalid file path is provided.
    /// The test ensures that no file is created at an invalid file path and the method gracefully handles the error scenario.
    /// </summary>
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

    /// <summary>
    /// Tests the WriteCsv method to ensure that it correctly handles fields containing quotes
    /// by properly escaping and enclosing them as needed in the generated CSV file.
    /// </summary>
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
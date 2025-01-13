using JetBrains.Annotations;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Sql.Context;
using Serilog;

namespace MyExpenses.Core.UnitTests.Export
{
    [TestSubject(typeof(ExistingDatabaseToFolder))]
    public class ExistingDatabaseToFolderTest
    {
        /// <summary>
        /// Validates that the specified folder path is created when exporting the database to a folder.
        /// </summary>
        /// <returns>Asynchronous task that completes successfully when the folder path is created.</returns>
        [Fact]
        public async Task ToFolderAsync_ShouldCreateSpecifiedFolderPath()
        {
            // Arrange
            var existingDatabase = new ExistingDatabase("TestDatabase.sqlite");

            const string folderPath = "OutputPath";
            const bool isCompress = false;

            // Act
            _ = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            var expectedFolder = Path.Combine(folderPath, existingDatabase.FileNameWithoutExtension);
            Assert.True(Directory.Exists(expectedFolder), "The folder should be created.");
        }

        [Fact]
        public async Task ToFolderAsync_ShouldExportRecordsToExcel()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseSqlite("DataSource=:memory:;Mode=Memory;Cache=Shared;")
                .LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging()
                .UseSeeding((dbContext, b) =>
                {
                    TestDatabaseSeeder.Seed(dbContext);
                })
                .Options;

            var context = new DataBaseContext(options);
            if (context.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
            {
                await context.Database.GetDbConnection().OpenAsync();
            }
            context.Database.EnsureCreated();

            var existingDatabase = new ExistingDatabase("MockDatabase.sqlite");

            const string folderPath = "OutputPath";
            const bool isCompress = false;

            var saveExcel = Path.Combine(folderPath, $"{existingDatabase.FileNameWithoutExtension}.xlsx");

            // Act
            var result = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            Assert.True(File.Exists(saveExcel), "The Excel file should be created.");
        }
        //
        // // Test for handling exceptions
        // [Fact]
        // public async Task ToFolderAsync_ShouldReturnFalse_OnException()
        // {
        //     // Arrange
        //     var existingDatabaseMock = new Mock<ExistingDatabase>();
        //     existingDatabaseMock.Setup(e => e.FilePath).Throws(new Exception("Test exception"));
        //
        //     var folderPath = "OutputPath";
        //     var isCompress = false;
        //
        //     // Act
        //     var result = await existingDatabaseMock.Object.ToFolderAsync(folderPath, isCompress);
        //
        //     // Assert
        //     Assert.False(result, "The method should return false when an exception is thrown.");
        // }
        //
        // // Test to confirm correct log messages are written during the process
        // [Fact]
        // public async Task ToFolderAsync_ShouldLogInformationDuringProcess()
        // {
        //     // Arrange
        //     var existingDatabase = new ExistingDatabase
        //     {
        //         FileNameWithoutExtension = "TestDatabase",
        //         FilePath = "TestPath"
        //     };
        //
        //     var folderPath = "OutputPath";
        //     var isCompress = false;
        //
        //     // Use a mock logger (or verify via Serilog test sinks)
        //     var loggerMock = new Mock<ILogger>();
        //
        //     // Act
        //     var result = await existingDatabase.ToFolderAsync(folderPath, isCompress);
        //
        //     // Verify if specific log messages are written
        //     loggerMock.Verify(
        //         x => x.Information(It.IsAny<string>(), It.IsAny<object[]>()),
        //         Times.AtLeastOnce);
        // }
        //
        // // Test for handling KML and GeoJSON generation
        // [Fact]
        // public async Task ToFolderAsync_ShouldExportRecordsToKmlAndGeoJson()
        // {
        //     // Arrange
        //     var existingDatabase = new ExistingDatabase
        //     {
        //         FileNameWithoutExtension = "TestDatabase",
        //         FilePath = "TestPath"
        //     };
        //
        //     var folderPath = "OutputPath";
        //     var isCompress = false;
        //
        //     var saveKml = Path.Combine(folderPath, $"{existingDatabase.FileNameWithoutExtension}.kmz");
        //     var saveGeoJson = Path.Combine(folderPath, $"{existingDatabase.FileNameWithoutExtension}.geojson");
        //
        //     // Act
        //     var result = await existingDatabase.ToFolderAsync(folderPath, isCompress);
        //
        //     // Assert
        //     Assert.True(File.Exists(saveKml), "The KML file should be created.");
        //     Assert.True(File.Exists(saveGeoJson), "The GeoJSON file should be created.");
        // }
    }
}
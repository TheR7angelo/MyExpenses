using JetBrains.Annotations;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;

namespace MyExpenses.Core.UnitTests.Export
{
    [TestSubject(typeof(ExistingDatabaseToFolder))]
    public class ExistingDatabaseToFolderTest
    {
        private static ExistingDatabase GetExistingDatabase()
        {
            var unitTestDbFilePath = Path.GetFullPath("UnitTestDb.sqlite");
            var existingDatabase = new ExistingDatabase(unitTestDbFilePath);

            return existingDatabase;
        }

        private static string GetOutputPath()
            => Path.GetFullPath("OutputPath");

        /// <summary>
        /// Validates that the specified folder path is created when exporting the database to a folder.
        /// </summary>
        /// <returns>Asynchronous task that completes successfully when the folder path is created.</returns>
        [Fact]
        public async Task ToFolderAsync_ShouldCreateSpecifiedFolderPath()
        {
            // Arrange
            var existingDatabase = GetExistingDatabase();

            var folderPath = GetOutputPath();
            const bool isCompress = false;

            // Act
            _ = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            var expectedFolder = Path.Combine(folderPath, existingDatabase.FileNameWithoutExtension);
            Assert.True(Directory.Exists(expectedFolder), "The folder should be created");
        }

        [Fact]
        public async Task ToFolderAsync_ShouldExportRecordsToExcel()
        {
            // Arrange
            var existingDatabase = GetExistingDatabase();

            var folderPath = GetOutputPath();
            const bool isCompress = false;

            var saveExcel = Path.Combine(folderPath, $"{existingDatabase.FileNameWithoutExtension}", $"{existingDatabase.FileNameWithoutExtension}.xlsx");

            // Act
            _ = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            Assert.True(File.Exists(saveExcel), "The Excel file should be created");
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
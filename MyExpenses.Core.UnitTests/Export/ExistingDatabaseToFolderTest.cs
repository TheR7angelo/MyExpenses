using JetBrains.Annotations;
using MyExpenses.Core.Export;
using MyExpenses.Models.IO;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;

namespace MyExpenses.Core.UnitTests.Export
{
    [TestSubject(typeof(ExistingDatabaseToFolder))]
    public class ExistingDatabaseToFolderTest
    {
        private static ExistingDatabase GetExistingDatabase()
        {
            var uuid = Guid.NewGuid().ToString();

            var unitTestDbFilePath = Path.GetFullPath("UnitTestDb.sqlite");

            var directoryName = Path.GetDirectoryName(unitTestDbFilePath)!;
            var newDbFilePath = Path.Combine(directoryName, $"{uuid}.sqlite");

            File.Copy(unitTestDbFilePath, newDbFilePath, overwrite: true);

            var existingDatabase = new ExistingDatabase(newDbFilePath);

            return existingDatabase;
        }

        private static void DeleteExistingDatabase(ExistingDatabase existingDatabase)
            => File.Delete(existingDatabase.FilePath);


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

            DeleteExistingDatabase(existingDatabase);
        }

        /// <summary>
        /// Validates that records from the existing database are exported to an Excel file in the specified folder path.
        /// </summary>
        /// <returns>Asynchronous task that completes successfully when the Excel file is created.</returns>
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

            DeleteExistingDatabase(existingDatabase);
        }

        /// <summary>
        /// Validates that the method returns false when an exception is thrown while attempting to export the database to a specified folder.
        /// </summary>
        /// <returns>Asynchronous task that completes successfully, returning false when an exception occurs during the export process.</returns>
        [Fact]
        public async Task ToFolderAsync_ShouldReturnFalse_OnException()
        {
            // Arrange
            var existingDatabase = new ExistingDatabase("DoNotExist.sqlite");

            var folderPath = GetOutputPath();
            const bool isCompress = false;

            // Act
            var result = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            Assert.False(result, "The method should return false when an exception is thrown");
        }

        /// <summary>
        /// Verifies that information-level log messages are recorded during the process of exporting an existing database to a folder.
        /// </summary>
        /// <returns>Asynchronous task that completes successfully when the log messages are validated.</returns>
        [Fact]
        public async Task ToFolderAsync_ShouldLogInformationDuringProcess()
        {
            // Arrange
            var existingDatabase = GetExistingDatabase();

            var folderPath = GetOutputPath();
            const bool isCompress = false;

            var testSink = InMemorySink.Instance;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Sink(testSink)
                .CreateLogger();

            // Act
            _ = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Verify if specific log messages are written
            Assert.Contains(testSink.LogEvents, logEvent => logEvent.Level is LogEventLevel.Information);
            Assert.Equal(6, testSink.LogEvents.Count(log => log.Level is LogEventLevel.Information));

            DeleteExistingDatabase(existingDatabase);
        }

        /// <summary>
        /// Validates that records are successfully exported to KML and GeoJSON file formats
        /// when exporting the database to a folder.
        /// </summary>
        /// <returns>Asynchronous task that completes successfully when KML and GeoJSON files are created.</returns>
        [Fact]
        public async Task ToFolderAsync_ShouldExportRecordsToKmlAndGeoJson()
        {
            // Arrange
            var existingDatabase = GetExistingDatabase();

            var folderPath = GetOutputPath();
            const bool isCompress = false;

            var saveKml = Path.Join(folderPath, $"{existingDatabase.FileNameWithoutExtension}", $"{existingDatabase.FileNameWithoutExtension}.kml");
            var saveGeoJson = Path.Join(folderPath, $"{existingDatabase.FileNameWithoutExtension}", $"{existingDatabase.FileNameWithoutExtension}.geojson");

            // Act
            _ = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            Assert.True(File.Exists(saveKml), "The KML file should be created.");
            Assert.True(File.Exists(saveGeoJson), "The GeoJSON file should be created.");
        }

        [Fact]
        public async Task ToFolderAsync_ShouldExportRecordsToKmz()
        {
            // Arrange
            var existingDatabase = GetExistingDatabase();

            var folderPath = GetOutputPath();
            const bool isCompress = true;

            var saveKml = Path.Join(folderPath, $"{existingDatabase.FileNameWithoutExtension}", $"{existingDatabase.FileNameWithoutExtension}.kmz");

            // Act
            _ = await existingDatabase.ToFolderAsync(folderPath, isCompress);

            // Assert
            Assert.True(File.Exists(saveKml), "The KMZ file should be created.");
        }
    }
}
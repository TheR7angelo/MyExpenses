using MyExpenses.IO.Excel;
using MyExpenses.IO.Sig.GeoJson;
using MyExpenses.IO.Sig.Kml;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;
using Serilog;

namespace MyExpenses.Core.Export;

public static class ExistingDatabaseToFolder
{
    /// <summary>
    /// Exports the content of the specified ExistingDatabase object to a folder.
    /// Creates a folder with the same name as the database file (excluding its extension) inside the specified folder path.
    /// Can optionally compress the exported data.
    /// </summary>
    /// <param name="existingDatabase">
    /// The ExistingDatabase object representing the database to be exported.
    /// </param>
    /// <param name="folderPath">
    /// The directory where the database content should be exported to.
    /// </param>
    /// <param name="isCompress">
    /// A boolean indicating whether the exported data should be compressed.
    /// </param>
    /// <returns>
    /// Returns a Task containing a boolean value indicating the success of the operation:
    /// True if the operation succeeds, False if the operation fails, such as when an exception occurs.
    /// </returns>
    public static async Task<bool> ToFolderAsync(this ExistingDatabase existingDatabase, string folderPath,
        bool isCompress)
    {
        Directory.CreateDirectory(folderPath);

        var saveFolder = Path.Join(folderPath, existingDatabase.FileNameWithoutExtension);
        if (Directory.Exists(saveFolder)) Directory.Delete(saveFolder, true);
        Directory.CreateDirectory(saveFolder);

        try
        {
            var saveExcel = Path.Join(saveFolder, $"{existingDatabase.FileNameWithoutExtension}.xlsx");
            Log.Information("Exporting records to Excel file at \"{SaveExcel}\"", saveExcel);

            // ReSharper disable once HeapView.ObjectAllocation.Evident
            // The allocation here is necessary as a new instance of DataBaseContext is required to interact with the database for this operation.
            // Since this instance is created within an async context and properly disposed with 'await using', it does not cause any unnecessary resource usage or performance issues.
            await using var context = new DataBaseContext(existingDatabase.FilePath);
            var resultExportToExcel = context.ToExcelWorksheet(saveExcel);

            if (resultExportToExcel) Log.Information("Records have been successfully exported to Excel file");
            else Log.Error("Error while exporting records to Excel file");

            var places = context.TPlaces.ToList();

            var xmlExtension = isCompress ? "kmz" : "kml";
            var saveKmz = Path.Join(saveFolder, $"{existingDatabase.FileNameWithoutExtension}.{xmlExtension}");
            Log.Information("Exporting records to Kml file at \"{SaveKmz}\"", saveKmz);
            var resultExportToKmlFile = places.ToKmlFile(saveKmz);

            if (resultExportToKmlFile) Log.Information("Records have been successfully exported to Kml file");
            else Log.Error("Error while exporting records to Kml file");

            var saveGeoJson = Path.Join(saveFolder, $"{existingDatabase.FileNameWithoutExtension}.geojson");
            Log.Information("Exporting records to GeoJson file at \"{SaveGeoJson}\"", saveGeoJson);
            // TODO add validation
            places.ToGeoJson(saveGeoJson);
            Log.Information("Records have been successfully exported to geojson file");

            // TODO work
            AddQgisProject(saveFolder);

            var finalResult = resultExportToExcel && resultExportToKmlFile;
            return finalResult;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while exporting records");
            return false;
        }
    }

    private static void AddQgisProject(string saveFolder)
    {
        const string qgisProjectFilename = "GeoVisionary.qgz";

        var resoucesDirectory = Path.GetFullPath("Resources");

        var qgisDirectory = Path.Join(resoucesDirectory, "Qgis");
        var qgisProjectFilePath = Path.Join(qgisDirectory, qgisProjectFilename);
        File.Copy(qgisProjectFilePath, Path.Join(saveFolder, qgisProjectFilename), true);

        var assetsDirectory = Path.Join(saveFolder, "Assets");
        Directory.CreateDirectory(assetsDirectory);

        var mapsDirectory = Path.Join(resoucesDirectory, "Assets", "Maps");
        var bleuMarkerFilePath = Path.Join(mapsDirectory, "BlueMarker.svg");
        var greenMarkerFilePath = Path.Join(mapsDirectory, "GreenMarker.svg");
        var redMarkerFilePath = Path.Join(mapsDirectory, "RedMarker.svg");

        ReadOnlySpan<string> svgs = [bleuMarkerFilePath, greenMarkerFilePath, redMarkerFilePath];
        foreach (var svg in svgs)
        {
            var filename = Path.GetFileName(svg);
            var newFilePath = Path.Join(assetsDirectory, filename);
            File.Copy(svg, newFilePath, true);
        }
    }
}
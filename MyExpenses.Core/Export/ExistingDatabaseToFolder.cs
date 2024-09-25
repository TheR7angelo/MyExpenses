using MyExpenses.IO.Excel;
using MyExpenses.IO.Sig.Kml;
using MyExpenses.Models.IO;
using MyExpenses.Sql.Context;
using Serilog;

namespace MyExpenses.Core.Export;

public static class ExistingDatabaseToFolder
{
    // TODO rework export Qgis
    public static async Task<bool> ToFolderAsync(this ExistingDatabase existingDatabase, string folderPath, bool isCompress)
    {
        Directory.CreateDirectory(folderPath);

        var saveFolder = Path.Join(folderPath, existingDatabase.FileNameWithoutExtension);
        if (Directory.Exists(saveFolder)) Directory.Delete(saveFolder, true);
        Directory.CreateDirectory(saveFolder);

        try
        {
            // Log.Information("Getting all records from all tables");
            // var exportRecords = existingDatabase.GetExportRecords();
            // Log.Information("All records have been recovered");
            //
            // Log.Information("Use of {ProcessorCount} processor for export", Environment.ProcessorCount);
            // var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            //
            // await Parallel.ForEachAsync(exportRecords, parallelOptions, (exportRecord, _) =>
            // {
            //     var name = exportRecord.Name;
            //     var records = exportRecord.Records;
            //
            //     var filePath = Path.Combine(saveFolder, name);
            //
            //     var isGeom = exportRecord.Source.GetInterfaces().Contains(typeof(ISig));
            //     if (isGeom)
            //     {
            //         var recordGeoms = records.Where(x => x is ISig)
            //             .Cast<ISig>()
            //             .Where(s => s.Geometry is not null)
            //             .ToList();
            //
            //         var geomType = recordGeoms.First().Geometry!.GetType().Name;
            //
            //         filePath = isCompress ? $"{filePath}.kmz" : $"{filePath}.kml";
            //         Log.Information("Exporting {Name} to kml file at \"{FilePath}\"", name, filePath);
            //
            //         recordGeoms.ToKmlFile(filePath, geomType);
            //     }
            //     // else
            //     // {
            //     //     filePath = Path.ChangeExtension(filePath, ".csv");
            //     //     Log.Information("Exporting {Name} to csv file at \"{FilePath}\"", name, filePath);
            //     //     records.WriteCsv(filePath);
            //     // }
            //     return default;
            // });

            var saveExcel = Path.Join(saveFolder, $"{existingDatabase.FileNameWithoutExtension}.xlsx");
            Log.Information("Exporting records to Excel file at \"{SaveExcel}\"", saveExcel);

            await using var context = new DataBaseContext(existingDatabase.FilePath);
            context.ToExcelWorksheet(saveExcel);
            var place = context.TPlaces.AsEnumerable();
            var saveKmz = Path.Join(saveFolder, $"{existingDatabase.FileNameWithoutExtension}.kmz");
            place.ToKmlFile(saveKmz);

            // TODO work
            // AddQgisProject(isCompress, saveFolder);

            Log.Information("Records have been successfully exported");
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while exporting records");
            return false;
        }
    }

    private static void AddQgisProject(bool isCompress, string saveFolder)
    {
        var resoucesDirectory = Path.GetFullPath("Resources");

        var qgisDirectory = Path.Join(resoucesDirectory, "Qgis");
        const string qgisProjectFilename = "GeoVisionary";
        var qgisProjectFilePath = Path.Join(qgisDirectory, $"{qgisProjectFilename}.qgs");
        const string qgisProjectAttachmentsFilename = "GeoVisionary_attachments.zip";
        var qgisProjectAttachmentsFilePath = Path.Join(qgisDirectory, qgisProjectAttachmentsFilename);

        // TODO add .qgz
        if (!isCompress)
        {
            var newQgisProjectFilePath = Path.Join(saveFolder, $"{qgisProjectFilename}.qgs");
            File.Copy(qgisProjectFilePath, newQgisProjectFilePath, true);

            var newQgisProjectAttachmentsFilePath = Path.Join(saveFolder, qgisProjectAttachmentsFilename);
            File.Copy(qgisProjectAttachmentsFilePath, newQgisProjectAttachmentsFilePath, true);
        }

        var assetsDirectory = Path.Join(saveFolder, "Assets");
        Directory.CreateDirectory(assetsDirectory);

        var mapsDirectory = Path.Join(resoucesDirectory, "Maps");
        var bleuMarkerFilePath = Path.Join(mapsDirectory, "BlueMarker.svg");
        var greenMarkerFilePath = Path.Join(mapsDirectory, "GreenMarker.svg");
        var redMarkerFilePath = Path.Join(mapsDirectory, "RedMarker.svg");
        var svgs = new List<string> { bleuMarkerFilePath, greenMarkerFilePath, redMarkerFilePath };
        foreach (var svg in svgs)
        {
            var filename = Path.GetFileName(svg);
            var newFilePath = Path.Join(assetsDirectory, filename);
            File.Copy(svg, newFilePath, true);
        }
    }
}
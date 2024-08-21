using MyExpenses.IO.Csv;
using MyExpenses.IO.Sig.Kml;
using MyExpenses.Models.IO;
using MyExpenses.Models.IO.Sig.Interfaces;
using Serilog;

namespace MyExpenses.Core.Export;

public static class ExistingDatabaseToFolder
{
    public static async Task<bool> ToFolderAsync(this ExistingDatabase existingDatabase, string folderPath)
    {
        try
        {
            Directory.CreateDirectory(folderPath);
            
            Log.Information("Getting all records from all tables");
            var exportRecords = existingDatabase.GetExportRecords();
            Log.Information("All records have been recovered");

            Log.Information("Use of {ProcessorCount} processor for export", Environment.ProcessorCount);
            var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
            
            await Parallel.ForEachAsync(exportRecords, parallelOptions, (exportRecord, _) =>
            {
                var name = exportRecord.Name;
                var records = exportRecord.Records;
                
                var filePath = Path.Combine(folderPath, name);
                
                var isGeom = exportRecord.Source.GetInterfaces().Contains(typeof(ISig));
                if (isGeom)
                {
                    var recordGeoms = records.Where(x => x is ISig)
                        .Cast<ISig>()
                        .Where(s => s.Geometry is not null)
                        .ToList();
                
                    var geomType = recordGeoms.First().Geometry!.GetType().Name;
                
                    var savePath = $"{filePath}.kml";
                    Log.Information("Exporting {Name} to kml file at \"{SavePath}\"", name, savePath);
                
                    recordGeoms.ToKmlFile(savePath, geomType);
                }
                else
                {
                    filePath = Path.ChangeExtension(filePath, ".csv");
                    Log.Information("Exporting {Name} to csv file at \"{FilePath}\"", name, filePath);
                    records.WriteCsv(filePath);
                }
                return default;
            });

            // TODO add .qgs
            return true;
        }
        catch (Exception e)
        {
            Log.Error(e, "Error while exporting records");
            return false;
        }
    }
}
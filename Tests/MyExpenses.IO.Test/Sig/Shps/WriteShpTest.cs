using System.Reflection;
using MyExpenses.IO.Sig.Shp;
using MyExpenses.SharedUtils.Utils;
using MyExpenses.Sql.Context;

namespace MyExpenses.IO.Test.Sig.Shps;

public class WriteShpTest
{
    public static string FileSavePath { get; } = Path.GetFullPath("test.shp");

    [Fact]
    public void WriteShp()
    {
        var executablePath = Assembly.GetExecutingAssembly().Location;
        var path = executablePath.GetParentDirectory(6);
        var dbFile = Path.Join(path, "MyExpenses.Wpf", "bin", "Debug", "net8.0-windows", "Databases", "Model - Using.sqlite");

        using var context = new DataBaseContext(dbFile);
        var projection = context.TSpatialRefSys.First(s => s.Srid.Equals(4326));
        var features = context.TPlaces.ToList();

        features.ToShapeFile(FileSavePath, projection.Srtext);

        var exist = File.Exists(FileSavePath);
        Assert.True(exist);
    }
}
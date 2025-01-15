using JetBrains.Annotations;
using MyExpenses.IO.Sig.Shp;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.IO.UnitTests.Sig.Shp;

[TestSubject(typeof(ShapeWriter))]
public class ShapeWriterTest
{
    private static readonly TPlace Place1 = new()
    {
        Id = 1,
        Name = "Statue of Liberty",
        Latitude = 40.6892,
        Longitude = -74.0445,
        Country = "USA",
        Street = "Liberty Island",
        City = "New York",
        Postal = "10004"
    };

    private static readonly TPlace Place2 = new()
    {
        Id = 2,
        Name = "Golden Gate Bridge",
        Latitude = 37.8199,
        Longitude = -122.4783,
        Country = "USA",
        Street = "Golden Gate",
        City = "San Francisco",
        Postal = "94129"
    };

    private static readonly TPlace Place3 = new()
    {
        Id = 3,
        Name = "Mount Rushmore",
        Latitude = 43.8791,
        Longitude = -103.4591,
        Country = "USA",
        Street = "13000 SD-244",
        City = "Keystone",
        Postal = "57751"
    };

    private static List<TPlace> GetTestRecords()
        => [Place1, Place2, Place3];

    private static string GetTempFilePath()
    {
        var tempPath = Path.GetTempPath();
        var guid = Guid.NewGuid().ToString();
        var filePath = Path.Join(tempPath, $"{guid}");
        return filePath;
    }

        [Fact]
    public void ToShapeFile_ShouldReturnTrue_WhenUSPointsAreValid()
    {
        // Arrange
        var places = GetTestRecords();

        var directory = GetTempFilePath();
        Directory.CreateDirectory(directory);

        var savePath = Path.Join(directory, "test_shapefile_us");
        const string projection = "PROJCS[\"WGS_1984_UTM_Zone_33N\"]";

        // Act
        var result = places.ToShapeFile(savePath, projection);

        // Assert
        Assert.True(result);
        foreach (var ext in new[] { "shp", "shx", "dbf", "prj", "cpg" })
        {
            var filePath = Path.ChangeExtension(savePath, ext);
            Assert.True(File.Exists(filePath));
        }

        // Cleanup
        Directory.Delete(directory, true);
    }

    [Fact]
    public void ToShapeFile_ShouldReturnFalse_WhenUSPointsCollectionIsEmpty()
    {
        // Arrange
        var places = new List<TPlace>(); // Collection vide

        var directory = GetTempFilePath();

        var savePath = Path.Join(directory, "test_shapefile_us");

        // Act
        var result = places.ToShapeFile(savePath);

        // Assert
        Assert.False(result);
    }
}
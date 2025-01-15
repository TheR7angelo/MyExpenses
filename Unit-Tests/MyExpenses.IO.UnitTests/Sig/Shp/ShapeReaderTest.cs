using JetBrains.Annotations;
using MyExpenses.IO.Sig.Shp;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.IO.UnitTests.Sig.Shp;

[TestSubject(typeof(ShapeReader))]
public class ShapeReaderTest
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
    public void ReadShapeFile_ShouldReturnFeaturesAndProjection()
    {
        // Arrange
        var directory = GetTempFilePath();
        Directory.CreateDirectory(directory);

        var shapefilePath = Path.Join(directory, "test_shapefile.shp");

        const string expectedProjection = "PROJCS[\"WGS 84 / UTM zone 33N\", GEOGCS[\"WGS 84\"]]";

        var records = GetTestRecords();
        records.ToShapeFile(shapefilePath, expectedProjection);

        // Act
        var (features, projection) = shapefilePath.ReadShapeFile<TPlace>();

        // Assert
        Assert.NotNull(projection);
        Assert.Equal(expectedProjection, projection);

        Assert.NotNull(features);
        Assert.Equal(records.Count, features.Count);

        for (var i = 0; i < features.Count; i++)
        {
            var expectedGeometry = records[i].Geometry;
            Assert.Equal(expectedGeometry, features[i].Geometry);
            Assert.Equal(records[i].Name, features[i].Name);
        }

        // Cleanup
        Directory.Delete(directory, true);
    }
}
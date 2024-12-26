using MyExpenses.IO.Sig.Shp;
using MyExpenses.Models.Sql.Bases.Tables;

namespace MyExpenses.IO.Test.Sig.Shps;

public class ReadShpTest
{
    private static void Setup()
    {
        var writeShpTest = new WriteShpTest();
        writeShpTest.WriteShp();
    }

    [Fact]
    public void ReadShp()
    {
        Setup();

        var (features, projection) = WriteShpTest.FileSavePath.ReadShapeFile<TPlace>();

        Assert.NotNull(features);
        Assert.NotNull(projection);
        Assert.NotEmpty(features);
    }
}
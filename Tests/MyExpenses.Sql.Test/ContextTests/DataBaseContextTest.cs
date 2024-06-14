using Microsoft.EntityFrameworkCore;
using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Test.ContextTests;

public class DataBaseContextTest
{
    private readonly string _dbPathTest = Path.Join(Path.GetFullPath("Resources"), "Model - Using - Test.sqlite");

    [Fact]
    private void DataBaseContextDbContextOptionsTest()
    {
        DataBaseContext.FilePath = _dbPathTest;
        var contextOption = new DbContextOptions<DataBaseContext>();
        using var context = new DataBaseContext(contextOption);

        Assert.NotNull(context);
    }

    [Fact]
    private void DataBaseContextStringEmptyTest()
    {
        DataBaseContext.FilePath = _dbPathTest;
        using var context = new DataBaseContext();

        Assert.NotNull(context);
    }

    [Fact]
    private void DataBaseContextStringNotEmptyTest()
    {
        using var context = new DataBaseContext(_dbPathTest);

        Assert.NotNull(context);
    }
}
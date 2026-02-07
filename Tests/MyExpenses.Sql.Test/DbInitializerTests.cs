using MyExpenses.Sql.Context;

namespace MyExpenses.Sql.Test;

public class DbInitializerTests
{
    [Fact]
    public void Initialize_ShouldCreateDatabaseAndAddDefaultData()
    {
        using var context = new DataBaseContext();

        var dataBaseSeeder = new DataBaseSeeder(context);
        dataBaseSeeder.SeedAll();
    }
}
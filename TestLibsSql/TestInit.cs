using System.Globalization;
using LibsSql;

namespace TestLibsSql;

public class TestInit
{
    [Fact]
    private void Init()
    {
        // Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-001");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr-fr");
        
        const string db = "DataBase/test.sqlite";

        var sqlite = new Sqlite(db);
    }
}
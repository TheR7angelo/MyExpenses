using LibsSql.Tables;
using LibsSql.Tables.DefaultValue;
using SQLite;

namespace LibsSql;

public class Sqlite
{
    private readonly SQLiteConnection _connection;

    public Sqlite(string dbPath)
    {
        _connection = !File.Exists(dbPath) ? InitDataBase(dbPath) : new SQLiteConnection(dbPath, false);
        _connection.Execute("PRAGMA foreignkeys = ON");
    }

    private static SQLiteConnection InitDataBase(string dbPath)
    {
        var conn = CreateDataBase(dbPath);
        CreateDefaultTables(conn);
        InsertDefaultData(conn);

        return conn;
    }

    private static void InsertDefaultData(SQLiteConnection conn)
        => conn.InsertAll(GetDefault.GetDefaults());

    private static void CreateDefaultTables(SQLiteConnection connection)
    {
        var baseTables = new List<string>
            { Category.Definition, WalletType.Definition, PaymentMode.Definition, Localisation.Definition };

        var tables = new List<string> { Wallet.Definition };

        foreach (var command in new List<List<string>> { baseTables, tables }
                     .SelectMany(commands => commands))
        {
            connection.Execute(command);
        }
    }

    private static SQLiteConnection CreateDataBase(string dbPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        return new SQLiteConnection(dbPath, false);
    }
}
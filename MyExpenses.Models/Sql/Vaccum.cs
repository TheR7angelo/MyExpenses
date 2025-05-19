namespace MyExpenses.Models.Sql;

public class Vaccum
{
    public delegate void VaccumDatabaseEventHandler();

    public static event VaccumDatabaseEventHandler? VaccumDatabase;

    public static void OnVacuumDatabase()
        => VaccumDatabase?.Invoke();
}
namespace MyExpenses.Sql.Migrations;

public sealed class Migration_1_0_0_To_1_1_0 : IDatabaseMigration
{
    public Version From => new(1, 0, 0);
    public Version To => new(1, 1, 0);
    public string Command => @"DROP TABLE IF EXISTS t_supported_languages";
}
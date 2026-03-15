namespace MyExpenses.Sql.Migrations;

public interface IDatabaseMigration
{
    public Version From { get; }
    public Version To { get; }
    public bool ForeignKeyOff { get; }
    public string Command { get; }
}
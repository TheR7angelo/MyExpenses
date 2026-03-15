using MyExpenses.SharedUtils.Resources;

namespace MyExpenses.Sql.Context;

public class DataBaseSeeder(DataBaseContextOld contextOld)
{
    public static Version CurrentVersion { get; } = new(1, 2, 0);

    private readonly DataBaseContextOld _contextOld = contextOld;

    public void SeedAll()
    {
        SeedLanguages();
    }

    private void SeedLanguages()
    {
        var supportedCultures = LanguagesUtils.GetSupportedCultures();

    }
}
using MyExpenses.SharedUtils.Resources;

namespace MyExpenses.Sql.Context;

public class DataBaseSeeder(DataBaseContext context)
{
    public static Version CurrentVersion { get; } = new(1, 1, 0);

    private readonly DataBaseContext _context = context;

    public void SeedAll()
    {
        SeedLanguages();
    }

    private void SeedLanguages()
    {
        var supportedCultures = LanguagesUtils.GetSupportedCultures();

    }
}
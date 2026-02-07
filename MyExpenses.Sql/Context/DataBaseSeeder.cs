using System.Globalization;
using MyExpenses.SharedUtils.Resources;

namespace MyExpenses.Sql.Context;

public class DataBaseSeeder(DataBaseContext context)
{
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
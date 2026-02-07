using System.Globalization;

namespace MyExpenses.SharedUtils.Resources;

public static class LanguagesUtils
{
    /// <summary>
    /// Retrieves a collection of <see cref="CultureInfo"/> objects representing the cultures
    /// supported by the application, based on the presence of resource files.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <see cref="CultureInfo"/> objects for the supported cultures.
    /// </returns>
    public static IEnumerable<CultureInfo> GetSupportedCultures()
    {
        var files = Directory.GetFiles(Path.Join(AppContext.BaseDirectory), "MyExpenses.SharedUtils.resources.dll", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            var directory = Path.GetDirectoryName(file);
            var cultureInfoName = Path.GetFileName(directory);

            if (TryGetCultureInfo(cultureInfoName, out var cultureInfo))
            {
                yield return cultureInfo!;
            }
        }
    }

    /// <summary>
    /// Attempts to retrieve a <see cref="CultureInfo"/> object for the specified culture name.
    /// </summary>
    /// <param name="name">The name of the culture to retrieve. This parameter can be null or whitespace.</param>
    /// <param name="culture">
    /// When this method returns, contains the <see cref="CultureInfo"/> object associated with the specified
    /// culture name, if found; otherwise, null.
    /// </param>
    /// <returns>
    /// Returns <c>true</c> if a <see cref="CultureInfo"/> object for the specified culture name is successfully retrieved;
    /// otherwise, <c>false</c>.
    /// </returns>
    private static bool TryGetCultureInfo(string? name, out CultureInfo? culture)
    {
        culture = null;
        if (string.IsNullOrWhiteSpace(name)) return false;

        try
        {
            culture = CultureInfo.GetCultureInfo(name);
            return true;
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }
}
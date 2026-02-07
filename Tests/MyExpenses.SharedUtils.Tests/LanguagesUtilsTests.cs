using MyExpenses.SharedUtils.Resources;

namespace MyExpenses.SharedUtils.Tests;

public class LanguagesUtilsTests
{
    /// <summary>
    /// Validates that the <see cref="LanguagesUtils.GetSupportedCultures"/> method returns at least one supported culture.
    /// Ensures the returned collection is neither empty nor contains null values.
    /// </summary>
    [Fact]
    public void GetSupportedCultures_ShouldReturnAtLeastOneCulture()
    {
        var cultures = LanguagesUtils.GetSupportedCultures().ToArray();

        Assert.NotEmpty(cultures);
        Assert.All(cultures, Assert.NotNull);
    }

    /// <summary>
    /// Ensures that the <see cref="LanguagesUtils.GetSupportedCultures"/> method returns a collection
    /// that includes specific cultures, validating that the expected culture identifiers are present.
    /// </summary>
    [Fact]
    public void GetSupportedCultures_ShouldContainSpecificCultures()
    {
        var cultures = LanguagesUtils.GetSupportedCultures();
        var names = cultures.Select(c => c.Name);

        Assert.Contains(LanguagesUtils.DefaultCultureName, names);
    }

    /// <summary>
    /// Verifies that the <see cref="LanguagesUtils.GetSupportedCultures"/> method returns cultures
    /// with valid display names and native names. Ensures that these properties are non-empty
    /// and do not contain null or whitespace values.
    /// </summary>
    [Fact]
    public void GetSupportedCultures_ShouldReturnValidDisplayNames()
    {
        var cultures = LanguagesUtils.GetSupportedCultures();

        Assert.All(cultures, c => {
            Assert.False(string.IsNullOrWhiteSpace(c.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(c.NativeName));
        });
    }
}
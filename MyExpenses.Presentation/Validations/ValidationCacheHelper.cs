using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace MyExpenses.Presentation.Validations;

/// <summary>
/// Provides caching mechanisms for retrieving localized validation messages associated with
/// <see cref="ValidationAttribute"/> instances.
/// </summary>
public static class ValidationCacheHelper
{
    /// <summary>
    /// Internal cache for storing localized validation messages.
    /// This cache serves to optimize the retrieval of error message strings
    /// for <see cref="ValidationAttribute"/> instances by reducing repeated lookups
    /// of resources and improving performance in validation operations.
    /// </summary>
    private static readonly ConcurrentDictionary<string, string?> InternalMessageCache = new();

    /// <summary>
    /// Retrieves the internal error message associated with the given validation attribute.
    /// </summary>
    /// <param name="attr">The <see cref="ValidationAttribute"/> instance containing the error message resource type and resource name.</param>
    /// <returns>
    /// A string representing the internal error message if available, otherwise null.
    /// </returns>
    public static string? GetInternalMessage(ValidationAttribute attr)
    {
        if (attr.ErrorMessageResourceType == null || attr.ErrorMessageResourceName == null) return null;

        var cacheKey = $"{attr.ErrorMessageResourceType.FullName}.{attr.ErrorMessageResourceName}";

        return InternalMessageCache.GetOrAdd(cacheKey, static (_, attribute) =>
        {
            var prop = attribute.ErrorMessageResourceType?.GetProperty("ResourceManager",
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            var resManager = (ResourceManager?)prop?.GetValue(null);

            return resManager?.GetString(attribute.ErrorMessageResourceName!, CultureInfo.InvariantCulture);
        }, attr);
    }
}
namespace MyExpenses.SharedUtils;

public static class EnumerableExtensions
{
    /// <summary>
    /// Traite un IEnumerable et retourne chaque élément avec son index sous forme d'objet.
    /// </summary>
    /// <typeparam name="T">Le type des éléments dans l'énumérable.</typeparam>
    /// <param name="source">La collection source à traiter.</param>
    /// <returns>Une liste d'objets contenant l'index et l'élément correspondant.</returns>
    /// <exception cref="ArgumentNullException">Si la collection source est null.</exception>
    public static IEnumerable<(int Index, T Element)> WithIndex<T>(this IEnumerable<T> source)
        => source.Select((element, index) => (Index: index, Element: element));
}
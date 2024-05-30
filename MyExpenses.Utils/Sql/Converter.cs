using MyExpenses.Models.Sql;
using Newtonsoft.Json;

namespace MyExpenses.Utils.Sql;

public static class Converter
{
    /// <summary>
    /// Converts an ISql object to a JSON string representation.
    /// </summary>
    /// <param name="iSql">The ISql object to convert.</param>
    /// <returns>A JSON string representation of the ISql object.</returns>
    public static string ToJsonString(this ISql iSql)
        => JsonConvert.SerializeObject(iSql, Formatting.Indented);
}
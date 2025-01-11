namespace MyExpenses.Utils.Converters;

public static class ValueSymbolConverter
{
    public static string Convert(object?[] values)
    {
        var numericValue = 0d;
        var symbol = string.Empty;

        switch (values)
        {
            case { Length: > 0 } when values[0] is double dValue:
                numericValue = dValue;
                break;
            case { Length: > 0 } when values[0] != null:
                var result = double.TryParse(values[0]?.ToString(), out numericValue);
                if (!result) numericValue = 0d;
                break;
        }

        symbol = values switch
        {
            { Length: > 1 } when values[1] is string sValue => sValue,
            { Length: > 1 } when values[1] is not null => values[1]?.ToString() ?? string.Empty,
            _ => symbol
        };

        var valueFormatted = numericValue.ToString("F2");

        return !string.IsNullOrWhiteSpace(symbol) ? $"{valueFormatted} {symbol}" : valueFormatted;
    }
}
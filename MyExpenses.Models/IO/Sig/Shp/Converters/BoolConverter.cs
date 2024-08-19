namespace MyExpenses.Models.IO.Sig.Shp.Converters;

public static class BoolConverter
{
    public static bool ToBool(this object? value)
    {
        try
        {
            return value switch
            {
                bool boolValue => boolValue,
                string str => str != "0",
                int intValue => intValue != 0,
                double doubleValue => doubleValue != 0,
                _ => false
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
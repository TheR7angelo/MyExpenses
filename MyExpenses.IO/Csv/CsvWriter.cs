using System.Globalization;
using CsvHelper.Configuration;

namespace MyExpenses.IO.Csv;

public static class CsvWriter
{
    private static CsvConfiguration CsvConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
            Delimiter = ";"
        };
    }

    public static bool WriteCsv<T>(this IEnumerable<T> records, string filePath)
    {
        filePath = Path.ChangeExtension(filePath, ".csv");

        using var writer = new StreamWriter(filePath);
        using var csv = new CsvHelper.CsvWriter(writer, CsvConfiguration());

        try
        {
            csv.WriteRecords(records);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}
using System.Globalization;
using CsvHelper.Configuration;

namespace MyExpenses.IO.Csv;

public static class CsvWriter
{
    /// <summary>
    /// Creates and configures a CsvConfiguration instance for use with CSV operations.
    /// The configuration specifies global settings such as culture, new line characters, delimiter,
    /// and custom logic to determine when a field should be quoted.
    /// </summary>
    /// <returns>
    /// A CsvConfiguration instance with the specified settings applied.
    /// </returns>
    // The allocation of CsvConfiguration is necessary here to define the specific settings for CSV operations.
    // Since it is a scoped and lightweight object, its allocation occurs only when required and has no measurable impact on performance.
    private static CsvConfiguration CsvConfiguration()
    {
        const string delimiter = ";";
        const char quote = '"';

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            NewLine = Environment.NewLine,
            Delimiter = delimiter,
            ShouldQuote = args => args.Field is not null && (args.Field.Contains('\n') ||
                                                             args.Field.Contains(quote) ||
                                                             args.Field.Contains(delimiter))
        };
    }

    public static bool WriteCsv<T>(this IEnumerable<T> records, string filePath)
    {
        filePath = Path.ChangeExtension(filePath, ".csv");

        try
        {
            // The allocation of StreamWriter and CsvWriter is necessary to write the records to the CSV file.
            // Both objects are properly disposed of using 'using' statements, ensuring no unmanaged resources are leaked.
            // These allocations are lightweight and scoped to this operation, having no significant impact on overall performance.

            // ReSharper disable HeapView.ObjectAllocation.Evident
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvHelper.CsvWriter(writer, CsvConfiguration());
            // ReSharper restore HeapView.ObjectAllocation.Evident

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
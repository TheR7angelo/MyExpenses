﻿using System.Globalization;
using System.Text;
using CsvHelper.Configuration;
using Ude;

namespace MyExpenses.IO.Csv;

public static class CsvReader
{
    /// <summary>
    /// Provides the configuration options for CSV file operations used within the application.
    /// </summary>
    /// <remarks>
    /// This property is a static instance of <see cref="CsvHelper.Configuration.CsvConfiguration"/> preconfigured
    /// to use the invariant culture and to handle specific CSV parsing features. The configuration disables header
    /// validation and missing field handling while enabling automatic detection of the delimiter.
    /// </remarks>
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    private static CsvConfiguration CsvConfiguration { get; } = new (CultureInfo.InvariantCulture)
    {
        HeaderValidated = null,
        MissingFieldFound = null,
        DetectDelimiter = true
    };

    static CsvReader()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    /// <summary>
    /// Reads the contents of a CSV file and maps them to a collection of objects of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to which the CSV rows will be mapped.</typeparam>
    /// <param name="filePath">The file path of the CSV file to be read.</param>
    /// <returns>An <see cref="IEnumerable{T}"/> containing the mapped objects from the CSV file.</returns>
    public static IEnumerable<T> ReadCsv<T>(this string filePath)
    {
        var encodingName = GetEncodingName(filePath);
        var encoding = Encoding.GetEncoding(encodingName);

        using var streamReader = new StreamReader(filePath, encoding, true,
            new FileStreamOptions { Access = FileAccess.Read, Share = FileShare.ReadWrite });

        using var reader = new CsvHelper.CsvReader(streamReader, CsvConfiguration);
        var records = reader.GetRecords<T>();

        return records.ToList();
    }

    private static string GetEncodingName(string filePath)
    {
        using var fileStream = File.OpenRead(filePath);

        var detector = new CharsetDetector();
        detector.Feed(fileStream);
        detector.DataEnd();

        var encodingName = string.IsNullOrEmpty(detector.Charset)
            ? "UTF-8"
            : detector.Charset;
        return encodingName;
    }
}
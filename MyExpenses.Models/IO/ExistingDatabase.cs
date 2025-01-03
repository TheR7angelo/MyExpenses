using System.Security.Cryptography;

namespace MyExpenses.Models.IO;

public class ExistingDatabase
{
    /// <summary>
    /// Gets the full path of the database file.
    /// This property represents the location of the database file on the filesystem
    /// and is crucial for file operations such as retrieval, update, or validation.
    /// </summary>
    public string FilePath { get; }

    /// <summary>
    /// Gets the name of the database file including its extension.
    /// This property represents only the filename, excluding the directory path,
    /// and is primarily used for operations requiring the file's descriptive identifier.
    /// </summary>

    public string FileName { get; }

    /// <summary>
    /// Gets the name of the file without its extension.
    /// This property is derived from the <see cref="FileName"/> property and excludes the file extension to simplify file identification.
    /// It can be useful when working with files in contexts where the file extension is not needed.
    /// </summary>
    public string FileNameWithoutExtension { get; }

    private FileInfo? _fileInfo;

    /// <summary>
    /// Gets the <see cref="System.IO.FileInfo"/> instance associated with the file specified by the <see cref="FilePath"/> property.
    /// This property provides access to metadata and functions related to the file.
    /// The value is lazily initialized and cached for later access.
    /// If the file doesn't exist, some <see cref="System.IO.FileInfo"/> properties may return default values or throw exceptions.
    /// </summary>
    public FileInfo FileInfo
        => _fileInfo ??= new FileInfo(FilePath);

    private string? _hashContent;

    /// <summary>
    /// Gets the computed SHA256 hash of the file content.
    /// This property returns a hexadecimal string that represents the hash of the local
    /// file's content specified by the <see cref="FilePath"/> property.
    /// The value is lazily calculated and cached for later accesses.
    /// If the file doesn't exist, an empty string is returned.
    /// </summary>
    public string HashContent
        => _hashContent ??= ComputeHashContent();

    private string? _hashContentHashContentDropbox;

    public string HashContentDropbox
        => _hashContentHashContentDropbox ??= ComputeDropboxContentHash();

    public ExistingDatabase(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
    }

    /// <summary>
    /// Computes the SHA256 hash of the file content specified by the FilePath property.
    /// The computation is performed in chunks to optimize memory usage for large files.
    /// Returns the hash as a hexadecimal string.
    /// </summary>
    /// <returns>
    /// A string representing the SHA256 hash of the file content.
    /// Returns an empty string if the file doesn't exist.
    /// </returns>
    private string ComputeHashContent()
    {
        const int bufferSize = 4 * 1024 * 1024; // 4 Mo (4 194 304 octets)

        if (!File.Exists(FilePath)) return string.Empty;

        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(FilePath);

        var buffer = new byte[bufferSize];
        int bytesRead;

        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            sha256.TransformBlock(buffer, 0, bytesRead, null, 0);
        }

        sha256.TransformFinalBlock([], 0, 0);
        return BitConverter.ToString(sha256.Hash!).Replace("-", "").ToLowerInvariant();
    }

    /// <summary>
    /// Computes the Dropbox content hash of the file specified by the FilePath property.
    /// The method divides the file into 4 MB chunks, computes the SHA256 hash for each chunk,
    /// concatenates these hashes, and then computes a final SHA256 hash of the concatenated result.
    /// Returns the computed hash as a hexadecimal string.
    /// </summary>
    /// <returns>
    /// A string representing the Dropbox content hash of the file.
    /// Returns an empty string if the file doesn't exist or can't be read.
    /// </returns>
    private string ComputeDropboxContentHash()
    {
        const int blockSize = 4 * 1024 * 1024; // 4 Mo (4 194 304 octets)

        using var sha256 = SHA256.Create();
        using var fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var buffer = new byte[blockSize];
        int bytesRead;
        using var concatenatedHashStream = new MemoryStream();

        while ((bytesRead = fileStream.Read(buffer, 0, blockSize)) > 0)
        {
            var blockHash = sha256.ComputeHash(buffer, 0, bytesRead);

            concatenatedHashStream.Write(blockHash, 0, blockHash.Length);
        }

        concatenatedHashStream.Position = 0;
        var finalHash = sha256.ComputeHash(concatenatedHashStream);

        return BitConverter.ToString(finalHash).Replace("-", "").ToLowerInvariant();
    }

}
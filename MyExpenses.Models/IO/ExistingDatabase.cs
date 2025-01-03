using System.Security.Cryptography;
using MyExpenses.Models.WebApi.DropBox;

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

    /// <summary>
    /// Gets or sets the synchronization status of the database with the Dropbox storage.
    /// This property indicates whether the local database is synchronized, outdated, or in an unknown state,
    /// based on comparison with the remote Dropbox version.
    /// </summary>
    public SyncStatus SyncStatus { get; set; } = SyncStatus.Unknown;

    public ExistingDatabase(string filePath)
    {
        FilePath = filePath;
        FileName = Path.GetFileName(filePath);
        FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FilePath);
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
    public string GetDropboxContentHash()
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
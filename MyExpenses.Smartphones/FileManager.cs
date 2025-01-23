namespace MyExpenses.Smartphones;

public static class FileManager
{
    public static void AddAllFiles()
    {
        Task.WhenAll(AddDatabaseFileModels(), AddMapsMaker());
    }

    private static async Task AddMapsMaker()
    {
        var packageDirectory = Path.Join("Resources", "Maps");
        var storageDirectoryPath = Path.Join(FileSystem.AppDataDirectory, packageDirectory);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A static array of file names that are used exclusively in this method.
        // This approach keeps things simple and efficient since the files are fixed
        // and do not require any dynamic modifications or usage elsewhere.
        var files = new [] { "BlueMarker.svg", "GreenMarker.svg", "RedMarker.svg" };
        foreach (var file in files)
        {
            var packageFile = Path.Join(packageDirectory, file);
            var storageFile = Path.Join(storageDirectoryPath, file);

            await WritePackageFile(packageFile, storageFile);
        }
    }

    private static async Task AddDatabaseFileModels()
    {
        var packageFile = Path.Join("Database Models", "Model.sqlite");
        var storagePath = Path.Join(FileSystem.AppDataDirectory, packageFile);

        await WritePackageFile(packageFile, storagePath);
    }

    private static async Task WritePackageFile(string packageFile, string storagePath)
    {
        var packageExist = await FileSystem.Current.AppPackageFileExistsAsync(packageFile);
        if (!packageExist) return;

        var parentPath = Path.GetDirectoryName(storagePath)!;
        Directory.CreateDirectory(parentPath);

        await using var stream = await FileSystem.Current.OpenAppPackageFileAsync(packageFile);
        await using var fileStream = File.Create(storagePath);
        // ReSharper disable once MethodHasAsyncOverload
        stream.CopyTo(fileStream);
    }
}
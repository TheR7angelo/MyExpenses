namespace MyExpenses.Smartphones;

public class FileManager
{
    public void AddAllFiles()
    {
        AddDatabaseFileModels();
    }

    private void AddDatabaseFileModels()
    {
        var packageFile = Path.Join("Database Models", "Model.sqlite");
        var storagePath = Path.Join(FileSystem.AppDataDirectory, packageFile);

        WritePackageFile(packageFile, storagePath)
            .ConfigureAwait(false).GetAwaiter().GetResult();
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
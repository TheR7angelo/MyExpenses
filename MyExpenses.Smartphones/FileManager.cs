namespace MyExpenses.Smartphones;

public class FileManager
{
    public void AddAllFiles()
    {

    }

    private async Task WritePackageFile(string packageFile, string storagePath)
    {
        var packageExist = await FileSystem.Current.AppPackageFileExistsAsync(packageFile);
        if (!packageExist) return;

        await using var stream = await FileSystem.Current.OpenAppPackageFileAsync(packageFile);
        await using var fileStream = File.Create(storagePath);
        await stream.CopyToAsync(fileStream);
    }
}
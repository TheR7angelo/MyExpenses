using MyExpenses.Models.Wpf.Save;

namespace MyExpenses.Smartphones.ContentPages.SaveLocation;

public static class SaveLocationContentPageUtils
{
    public static async Task<Models.Wpf.Save.SaveLocation?> GetImportSaveLocation(this SaveLocationMode saveLocationMode)
        => await saveLocationMode.GetSaveLocation();

    public static async Task<Models.Wpf.Save.SaveLocation?> GetExportSaveLocation()
    {
        var saveLocation = await SaveLocationMode.LocalDropbox.GetSaveLocation();

        if (saveLocation is Models.Wpf.Save.SaveLocation.Dropbox) return saveLocation;
        if (saveLocation is null) return null;

        await Task.Delay(TimeSpan.FromMilliseconds(100));

        saveLocation = await SaveLocationMode.FolderFolderCompressDatabase.GetSaveLocation();
        return saveLocation;
    }

    private static async Task<Models.Wpf.Save.SaveLocation?> GetSaveLocation(this SaveLocationMode saveLocationMode)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of SaveLocationContentPage to present the save location dialog to the user.
        // This creates and pushes the page to the navigation stack, allowing the user to interact with the UI
        // and select a save location. The ResultDialog property is awaited to retrieve the user’s choice.
        var saveLocationContentPage = new SaveLocationContentPage(saveLocationMode);
        await saveLocationContentPage.NavigateToAsync();
        var result = await saveLocationContentPage.ResultDialog;

        return result is not true ? null : saveLocationContentPage.SaveLocationResult;
    }
}
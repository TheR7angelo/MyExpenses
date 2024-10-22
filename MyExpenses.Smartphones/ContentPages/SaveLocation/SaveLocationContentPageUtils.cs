using MyExpenses.Models.Wpf.Save;

namespace MyExpenses.Smartphones.ContentPages.SaveLocation;

public static class SaveLocationContentPageUtils
{
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
        var saveLocationContentPage = new SaveLocationContentPage(saveLocationMode);
        await Shell.Current.Navigation.PushAsync(saveLocationContentPage);
        var result = await saveLocationContentPage.ResultDialog;

        return result is not true ? null : saveLocationContentPage.SaveLocationResult;
    }
}
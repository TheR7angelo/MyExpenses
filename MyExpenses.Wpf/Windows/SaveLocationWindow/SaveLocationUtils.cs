using MyExpenses.Models.Wpf.Save;
using MyExpenses.Wpf.Resources.Resx.Windows.SaveLocationWindow;

namespace MyExpenses.Wpf.Windows.SaveLocationWindow;

public static class SaveLocationUtils
{
    public static SaveLocation? GetImportSaveLocation(SaveLocationMode saveLocationMode)
    {
        var title = SaveLocationWindowResources.ImportSaveLocationTitle;
        return title.GetSaveLocation(saveLocationMode);
    }

    public static SaveLocation? GetExportSaveLocation(SaveLocationMode saveLocationMode)
    {
        var title = SaveLocationWindowResources.ExportSaveLocationTitle;
        return title.GetSaveLocation(saveLocationMode);
    }

    private static SaveLocation? GetSaveLocation(this string? title, SaveLocationMode saveLocationMode)
    {
        var saveLocationWindow = new SaveLocationWindow(saveLocationMode) { Title = title };
        saveLocationWindow.ShowDialog();

        return saveLocationWindow.DialogResult is not true ? null : saveLocationWindow.SaveLocationResult!;
    }
}
using MyExpenses.Models.Wpf.Save;
using MyExpenses.Wpf.Resources.Resx.Windows.SaveLocationWindow;

namespace MyExpenses.Wpf.Windows.SaveLocationWindow;

public static class SaveLocationUtils
{
    public static SaveLocation? GetImportSaveLocation()
    {
        var title = SaveLocationWindowResources.ImportSaveLocationTitle;
        return title.GetSaveLocation();
    }

    public static SaveLocation? GetExportSaveLocation()
    {
        var title = SaveLocationWindowResources.ExportSaveLocationTitle;
        return title.GetSaveLocation();
    }

    private static SaveLocation? GetSaveLocation(this string? title)
    {
        var saveLocationWindow = new SaveLocationWindow { Title = title };
        saveLocationWindow.ShowDialog();

        if (saveLocationWindow.DialogResult is not true) return null;
        return saveLocationWindow.SaveLocationResult;
    }
}
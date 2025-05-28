using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Sql.Context;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class ColorManagementContentPage
{
    public ObservableCollection<TColor> Colors { get; } = [];

    public ColorManagementContentPage()
    {
        RefreshColors();

        InitializeComponent();
    }

    private void RefreshColors()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        Colors.Clear();
        Colors.AddRange(context.TColors.OrderBy(s => s.Name));
    }

    private void RefreshColor(TColor color, bool add = false, bool remove = false)
    {
        switch (add)
        {
            case true when remove:
                throw new ArgumentException("'add' and 'remove' cannot both be true at the same time.");
            case true:
                Colors.AddAndSort(color, s => s.Name!);
                break;
            default:
                Colors.Remove(color);
                break;
        }
    }

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Border border) return;
        if (border.BindingContext is not TColor color) return;

        _ = HandleAddEditColor(color);
    }

    private void ButtonAddColor_OnClick(object? sender, EventArgs e)
        => _ = HandleAddEditColor();

    private async Task HandleAddEditColor(TColor? color = null)
    {
        var colorPickerPopup = new ColorPickerPopup { EditColor = true };
        if (color is not null) colorPickerPopup.SetColor(color);
        await this.ShowPopupAsync(colorPickerPopup);

        var result = await colorPickerPopup.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        var hexadecimal = colorPickerPopup.BackgroundColor.ToArgbHex(true);
        var newColor = new TColor { Name = colorPickerPopup.ColorName, HexadecimalColorCode = hexadecimal };

        await HandleColorResult(result, newColor, color);

    }

    private async Task HandleColorResult(ECustomPopupEntryResult result, TColor newColor, TColor? oldColor)
    {
        if (result is ECustomPopupEntryResult.Delete) await HandleDeleteColor(oldColor!);
        else if (result is ECustomPopupEntryResult.Valid && oldColor is null) await HandleAddColor(newColor);
        else await HandleEditColor(newColor, oldColor!);
    }

    private async Task HandleDeleteColor(TColor oldColor)
    {
        var message = string.Format(ColorManagementResources.MessageBoxDeleteColorQuestionMessage, oldColor.Name);
        var response = await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorQuestionTitle, message,
            ColorManagementResources.MessageBoxDeleteColorQuestionYesButton, ColorManagementResources.MessageBoxDeleteColorQuestionNoButton);

        if (response is not true) return;

        Log.Information("Attempting to remove the color \"{ColorToDeleteName}\"", oldColor.Name);
        var (success, exception) = oldColor.Delete();

        if (success)
        {
            Log.Information("Color was successfully removed");
            await DisplayAlert("Success", ColorManagementResources.MessageBoxDeleteColorNoUseSuccess, "Ok");
            // MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorNoUseSuccess, MsgBoxImage.Check);

            RefreshColor(oldColor, remove: true);
            // DeleteColor = true;
            // DialogResult = true;
            // Close();
            return;
        }
        //
        // if (exception!.InnerException is SqliteException
        //     {
        //         SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
        //     })
        // {
        //     Log.Error("Foreign key constraint violation");
        //
        //     response = MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorUseQuestion,
        //         MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        //
        //     if (response is not MessageBoxResult.Yes) return;
        //
        //     Log.Information("Attempting to remove the color \"{ColorToDeleteName}\" with all relative element",
        //         Color.Name);
        //     Color.Delete(true);
        //     Log.Information("Account and all relative element was successfully removed");
        //     MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteColorUseSuccess, MsgBoxImage.Check);
        //
        //     DeleteColor = true;
        //     DialogResult = true;
        //     Close();
        //
        //     return;
        // }
        //
        // Log.Error(exception, "An error occurred please retry");
        // MsgBox.MsgBox.Show(ColorManagementResources.MessageBoxDeleteAccountError, MsgBoxImage.Error);
    }

    private async Task HandleAddColor(TColor newColor)
    {
        // TODO work
    }

    private async Task HandleEditColor(TColor newColor, TColor oldColor)
    {
        // TODO work
    }
}
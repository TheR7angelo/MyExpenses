using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.ColorManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
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

    #region Action

    private void ButtonAddColor_OnClick(object? sender, EventArgs e)
        => _ = HandleAddEditColor();

    private void TapGestureRecognizer_OnTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Border border) return;
        if (border.BindingContext is not TColor color) return;

        _ = HandleAddEditColor(color);
    }

    #endregion

    #region Function

    private bool CheckColorName(string colorName)
        => Colors.Select(s => s.Name).Contains(colorName);

    private async Task HandleAddColor(TColor newColor)
    {
        Log.Information("Attempt to inject the new color \"{ColorName}\" with hexadecimal code \"{ColorHexadecimalColorCode}\"",
            newColor.Name, newColor.HexadecimalColorCode);

        var (success, exception) = newColor.AddOrEdit();
        if (success)
        {
            Log.Information("color was successfully added");
            var json = newColor.ToJsonString();
            Log.Information("{Json}", json);

            await DisplayAlert(ColorManagementResources.MessageBoxAddColorSuccessTitle,
                ColorManagementResources.MessageBoxAddColorSuccessMessage,
                ColorManagementResources.MessageBoxAddColorSuccessOkButton);

            Colors.AddAndSort(newColor, s => s.Name!);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            await DisplayAlert(ColorManagementResources.MessageBoxAddColorErrorTitle,
                ColorManagementResources.MessageBoxAddColorErrorMessage,
                ColorManagementResources.MessageBoxAddColorErrorOkButton);
        }
    }

    private async Task HandleAddEditColor(TColor? color = null)
    {
        var colorPickerPopup = new ColorPickerPopup { EditColor = true };
        if (color is not null) colorPickerPopup.SetColor(color);
        await this.ShowPopupAsync(colorPickerPopup);

        var result = await colorPickerPopup.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        var hexadecimal = colorPickerPopup.BackgroundColor.ToArgbHex(true);
        var newColor = new TColor { Name = colorPickerPopup.ColorName, HexadecimalColorCode = hexadecimal };

        var newColorIsError = await NewColorIsError(newColor);
        if (newColorIsError) return;

        await HandleColorResult(result, newColor, color);

    }

    private async Task HandleColorResult(ECustomPopupEntryResult result, TColor newColor, TColor? oldColor)
    {
        switch (result)
        {
            case ECustomPopupEntryResult.Delete:
                await HandleDeleteColor(oldColor!);
                break;
            case ECustomPopupEntryResult.Valid when oldColor is null:
                await HandleAddColor(newColor);
                break;
            default:
                await HandleEditColor(newColor, oldColor!);
                break;
        }
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
            await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorNoUseSuccessTitle,
                ColorManagementResources.MessageBoxDeleteColorNoUseSuccessMessage,
                ColorManagementResources.MessageBoxDeleteColorNoUseSuccessOkButton);

            RefreshColor(oldColor, remove: true);
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorUseQuestionTitle,
                ColorManagementResources.MessageBoxDeleteColorUseQuestionMessage,
                ColorManagementResources.MessageBoxDeleteColorUseQuestionYesButton,
                ColorManagementResources.MessageBoxDeleteColorUseQuestionNoButton);

            if (response is not true) return;

            Log.Information("Attempting to remove the color \"{ColorToDeleteName}\" with all relative element",
                oldColor.Name);
            oldColor.Delete(true);
            Log.Information("Account and all relative element was successfully removed");
            await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorUseSuccessTitle,
                ColorManagementResources.MessageBoxDeleteColorUseSuccessMessage,
                ColorManagementResources.MessageBoxDeleteColorUseSuccessOkButton);

            RefreshColor(oldColor, remove: true);
            return;
        }

        Log.Error(exception, "An error occurred please retry");
        await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorErrorTitle,
            ColorManagementResources.MessageBoxDeleteColorErrorMessage,
            ColorManagementResources.MessageBoxDeleteColorErrorOkButton);
    }

    private async Task HandleEditColor(TColor newColor, TColor oldColor)
    {
        oldColor.Name = newColor.Name;
        oldColor.HexadecimalColorCode = newColor.HexadecimalColorCode;

        Log.Information("Attempting to edit the color \"{AccountName}\"", oldColor.Name);
        var (success, exception) = oldColor.AddOrEdit();
        if (success)
        {
            Log.Information("Color was successfully edited");
            var json = oldColor.ToJsonString();
            Log.Information("{Json}", json);

            await DisplayAlert(ColorManagementResources.MessageBoxEditColorSuccessTitle,
                ColorManagementResources.MessageBoxEditColorSuccessMessage,
                ColorManagementResources.MessageBoxEditColorSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            await DisplayAlert(ColorManagementResources.MessageBoxEditColorErrorTitle,
                ColorManagementResources.MessageBoxEditColorErrorMessage,
                ColorManagementResources.MessageBoxEditColorErrorOkButton);
        }
    }

    private async Task<bool> NewColorIsError(TColor newColor)
    {
        if (string.IsNullOrWhiteSpace(newColor.Name))
        {
            await DisplayAlert(ColorManagementResources.MessageBoxCannotAddEmptyColorNameErrorTitle,
                ColorManagementResources.MessageBoxCannotAddEmptyColorNameErrorMessage,
                ColorManagementResources.MessageBoxCannotAddEmptyColorNameErrorOkButton);
            return true;
        }

        if (string.IsNullOrWhiteSpace(newColor.HexadecimalColorCode))
        {
            await DisplayAlert(ColorManagementResources.MessageBoxCannotAddEmptyColorHexErrorTitle,
                ColorManagementResources.MessageBoxCannotAddEmptyColorHexErrorMessage,
                ColorManagementResources.MessageBoxCannotAddEmptyColorHexErrorOkButton);
            return true;
        }

        var nameAlreadyExist = CheckColorName(newColor.Name);
        if (nameAlreadyExist)
        {
            await ShowErrorMessageDuplicateName();
            return true;
        }

        // ReSharper disable once HeapView.DelegateAllocation
        var colorAlreadyExist = Colors.FirstOrDefault(s => s.HexadecimalColorCode == newColor.HexadecimalColorCode);
        if (colorAlreadyExist is not null)
        {
            var message = string.Format(ColorManagementResources.MessageBoxCannotAddDuplicateColorHexErrorMessage,
                colorAlreadyExist.Name);
            await DisplayAlert(ColorManagementResources.MessageBoxCannotAddDuplicateColorHexErrorTitle, message,
                ColorManagementResources.MessageBoxCannotAddDuplicateColorHexErrorOkButton);
            return true;
        }

        return false;
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

    private async Task ShowErrorMessageDuplicateName()
        => await DisplayAlert(ColorManagementResources.MessageBoxCannotAddDuplicateColorNameErrorTitle,
            ColorManagementResources.MessageBoxCannotAddDuplicateColorNameErrorMessage,
            ColorManagementResources.MessageBoxCannotAddDuplicateColorNameErrorOkButton);

    #endregion
}
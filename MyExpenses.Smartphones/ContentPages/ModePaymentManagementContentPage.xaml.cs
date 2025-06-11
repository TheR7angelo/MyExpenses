using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Views;
using MyExpenses.Models.Maui.CustomPopup;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Resources.Resx.ModePaymentManagement;
using MyExpenses.Smartphones.ContentPages.CustomPopups;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Sql;
using Serilog;

namespace MyExpenses.Smartphones.ContentPages;

public partial class ModePaymentManagementContentPage
{
    private int MaxLength { get; }

    public ObservableCollection<TModePayment> ModePayments { get; } = [];

    public ModePaymentManagementContentPage()
    {
        MaxLength = Utils.Converters.MaxLengthConverter.Convert(typeof(TModePayment), nameof(TModePayment.Name));
        RefreshModePayments();

        InitializeComponent();
    }

    #region Action

    private void ButtonAddModePayment_OnClick(object? sender, EventArgs e)
        => _ = HandleAddEditModePayment();

    private void ButtonModePayment_OnClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button) return;
        if (button.BindingContext is not TModePayment modePayment) return;
        _ = HandleAddEditModePayment(modePayment);
    }

    #endregion

    #region Function

    private bool CheckModePaymentName(string modePaymentName)
        => ModePayments.Select(s => s.Name).Contains(modePaymentName);

    private async Task HandleAddNewModePayment(TModePayment newModePayment)
    {
        Log.Information("Attempt to inject the new mode payment \"{ColorName}\"", newModePayment.Name);
        var (success, exception) = newModePayment.AddOrEdit();

        if (success)
        {
            Log.Information("mode payment was successfully added");
            var json = newModePayment.ToJsonString();
            Log.Information("{Json}", json);

            await DisplayAlert(ModePaymentManagementResources.MessageBoxAddModePaymentSuccessTitle,
                ModePaymentManagementResources.MessageBoxAddModePaymentSuccessMessage,
                ModePaymentManagementResources.MessageBoxAddModePaymentSuccessOkButton);

            ModePayments.AddAndSort(newModePayment, s => s.Name!);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            await DisplayAlert(ModePaymentManagementResources.MessageBoxAddModePaymentErrorTitle,
                ModePaymentManagementResources.MessageBoxAddModePaymentErrorMessage,
                ModePaymentManagementResources.MessageBoxAddModePaymentErrorOkButton);
        }
    }

    // private async Task HandleAddEditColor(TColor? color = null)
    // {
    //     var colorPickerPopup = new ColorPickerPopup { EditColor = true };
    //     if (color is not null) colorPickerPopup.SetColor(color);
    //     await this.ShowPopupAsync(colorPickerPopup);
    //
    //     var result = await colorPickerPopup.ResultDialog;
    //     if (result is ECustomPopupEntryResult.Cancel) return;
    //
    //     var hexadecimal = colorPickerPopup.BackgroundColor.ToArgbHex(true);
    //     var newColor = new TColor { Name = colorPickerPopup.ColorName, HexadecimalColorCode = hexadecimal };
    //
    //     var newColorIsError = await NewColorIsError(newColor);
    //     if (newColorIsError) return;
    //
    //     await HandleColorResult(result, newColor, color);
    //
    // }

    // private async Task HandleDeleteColor(TColor oldColor)
    // {
    //     var message = string.Format(ColorManagementResources.MessageBoxDeleteColorQuestionMessage, oldColor.Name);
    //     var response = await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorQuestionTitle, message,
    //         ColorManagementResources.MessageBoxDeleteColorQuestionYesButton, ColorManagementResources.MessageBoxDeleteColorQuestionNoButton);
    //
    //     if (response is not true) return;
    //
    //     Log.Information("Attempting to remove the color \"{ColorToDeleteName}\"", oldColor.Name);
    //     var (success, exception) = oldColor.Delete();
    //
    //     if (success)
    //     {
    //         Log.Information("Color was successfully removed");
    //         await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorNoUseSuccessTitle,
    //             ColorManagementResources.MessageBoxDeleteColorNoUseSuccessMessage,
    //             ColorManagementResources.MessageBoxDeleteColorNoUseSuccessOkButton);
    //
    //         RefreshColor(oldColor, remove: true);
    //     }
    //
    //     if (exception!.InnerException is SqliteException
    //         {
    //             SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
    //         })
    //     {
    //         Log.Error("Foreign key constraint violation");
    //
    //         response = await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorUseQuestionTitle,
    //             ColorManagementResources.MessageBoxDeleteColorUseQuestionMessage,
    //             ColorManagementResources.MessageBoxDeleteColorUseQuestionYesButton,
    //             ColorManagementResources.MessageBoxDeleteColorUseQuestionNoButton);
    //
    //         if (response is not true) return;
    //
    //         Log.Information("Attempting to remove the color \"{ColorToDeleteName}\" with all relative element",
    //             oldColor.Name);
    //         oldColor.Delete(true);
    //         Log.Information("Account and all relative element was successfully removed");
    //         await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorUseSuccessTitle,
    //             ColorManagementResources.MessageBoxDeleteColorUseSuccessMessage,
    //             ColorManagementResources.MessageBoxDeleteColorUseSuccessOkButton);
    //
    //         RefreshColor(oldColor, remove: true);
    //         return;
    //     }
    //
    //     Log.Error(exception, "An error occurred please retry");
    //     await DisplayAlert(ColorManagementResources.MessageBoxDeleteColorErrorTitle,
    //         ColorManagementResources.MessageBoxDeleteColorErrorMessage,
    //         ColorManagementResources.MessageBoxDeleteColorErrorOkButton);
    // }

    private async Task HandleEditModePayment(TModePayment newModePayment, TModePayment oldModePayment)
    {
        oldModePayment.Name = newModePayment.Name;

        Log.Information("Attempting to edit the mode payment \"{ModePaymentName}\"", oldModePayment.Name);
        var (success, exception) = oldModePayment.AddOrEdit();
        if (success)
        {
            Log.Information("Mode payment was successfully edited");
            var json = oldModePayment.ToJsonString();
            Log.Information("{Json}", json);

            // await DisplayAlert(ColorManagementResources.MessageBoxEditColorSuccessTitle,
                // ColorManagementResources.MessageBoxEditColorSuccessMessage,
                // ColorManagementResources.MessageBoxEditColorSuccessOkButton);
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            // await DisplayAlert(ColorManagementResources.MessageBoxEditColorErrorTitle,
                // ColorManagementResources.MessageBoxEditColorErrorMessage,
                // ColorManagementResources.MessageBoxEditColorErrorOkButton);
        }
    }

    private async Task<bool> NewModePaymentIsError(TModePayment newModePayment)
    {
        if (string.IsNullOrWhiteSpace(newModePayment.Name))
        {
            await DisplayAlert(ModePaymentManagementResources.MessageBoxModePaymentNameEmptyErrorTitle,
                ModePaymentManagementResources.MessageBoxModePaymentNameEmptyErrorMessage,
                ModePaymentManagementResources.MessageBoxModePaymentNameEmptyErrorOkButton);
            return true;
        }

        var nameAlreadyExist = CheckModePaymentName(newModePayment.Name);
        if (nameAlreadyExist)
        {
            await DisplayAlert(ModePaymentManagementResources.MessageBoxModePaymentNameAlreadyExistsTitle,
                ModePaymentManagementResources.MessageBoxModePaymentNameAlreadyExistsMessage,
                ModePaymentManagementResources.MessageBoxModePaymentNameAlreadyExistsOkButton);
            return true;
        }

        return false;
    }

    #endregion

    private void RefreshModePayment(TModePayment modePayment, bool add = false, bool remove = false)
    {
        switch (add)
        {
            case true when remove:
                throw new ArgumentException("'add' and 'remove' cannot both be true at the same time.");
            case true:
                ModePayments.AddAndSort(modePayment, s => s.Name!);
                break;
            default:
                ModePayments.Remove(modePayment);
                break;
        }
    }

    private void RefreshModePayments()
    {
        ModePayments.Clear();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The creation of a new DataBaseContext instance (via `new DataBaseContext()`) is necessary to interact with the database.
        // This context provides the connection to the database and allows querying or updating data.
        // The `using` statement ensures that the context is disposed of properly after its use, freeing up resources like database connections.
        using var context = new DataBaseContext();
        ModePayments.AddRange(context.TModePayments.OrderBy(s => s.Name));
    }

    private async Task HandleAddEditModePayment(TModePayment? modePayment = null)
    {
        var placeHolder = ModePaymentManagementResources.TextBoxModePaymentName;

        var modePaymentName = string.Empty;
        switch (modePayment?.CanBeDeleted)
        {
            case false:
                await DisplayAlert(ModePaymentManagementResources.MessageBoxModePaymentCantEditTitle,
                    ModePaymentManagementResources.MessageBoxModePaymentCantEditMessage,
                    ModePaymentManagementResources.MessageBoxModePaymentCantEditOkButton);
                return;
            case true:
                modePaymentName = modePayment.Name!;
                break;
        }

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // A new instance of CustomPopupEntry is created and initialized with specific properties such as MaxLenght,
        // PlaceholderText, EntryText, and CanDelete. This instance is configured to provide a customizable popup
        // for editing or interacting with a currency's symbol. This setup allows the user to input or modify data
        // interactively while maintaining flexibility and ensuring proper validation during the interaction.
        var customPopupEntry = new CustomPopupEntry
        {
            MaxLenght = MaxLength, PlaceholderText = placeHolder,
            EntryText = modePaymentName, CanDelete = modePayment?.CanBeDeleted ?? false
        };
        await this.ShowPopupAsync(customPopupEntry);

        var result = await customPopupEntry.ResultDialog;
        if (result is ECustomPopupEntryResult.Cancel) return;

        var newModePayment = new TModePayment { Name = customPopupEntry.EntryText, CanBeDeleted = true };

        var newModePaymentIsError = await NewModePaymentIsError(newModePayment);
        if (newModePaymentIsError) return;

        await HandleModePaymentResult(result, newModePayment, modePayment);
    }

    private async Task HandleModePaymentResult(ECustomPopupEntryResult result, TModePayment newModePayment, TModePayment? oldModePayment)
    {
        switch (result)
        {
            // case ECustomPopupEntryResult.Delete:
            //     await HandleDeleteColor(oldColor!);
            //     break;
            case ECustomPopupEntryResult.Valid when oldModePayment is null:
                await HandleAddNewModePayment(newModePayment);
                break;
            default:
                await HandleEditModePayment(newModePayment, oldModePayment!);
                break;
        }
    }
}
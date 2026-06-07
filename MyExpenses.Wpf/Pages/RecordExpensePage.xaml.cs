using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.DetailedRecordManagement;
using MyExpenses.SharedUtils.Resources.Resx.ModePaymentManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Windows;
using MyExpenses.Utils.Sql;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public THistory History { get; } = new();

    public ObservableCollection<TModePayment> ModePayments { get; }

    private ExpenseManagementViewModel ExpenseManagementViewModel
        => (ExpenseManagementViewModel)DataContext;

    public RecordExpensePage(ExpenseManagementViewModel vm)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContextOld();
        ModePayments = [..context.TModePayments.OrderBy(s => s.Name)];

        InitializeComponent();

        UpdateConfiguration();

        // ReSharper disable once HeapView.DelegateAllocation
        Configuration.ConfigurationChanged += Configuration_OnConfigurationChanged;

        DataContext = vm;
    }

    #region Action

    private void ButtonPlace_OnClick(object sender, RoutedEventArgs e)
    {
        // TODO correct
        // var place = History.PlaceFk?.ToISql<TPlace>();
        // if (place?.CanBeDeleted is false)
        // {
        //     MsgBox.Show(LocationManagementResources.MessageBoxPlaceCantEditMessage, MsgBoxImage.Error);
        //     return;
        // }
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // An instance of AddEditPlaceWindow is created to handle adding or editing a place.
        // // The place is set to null if the place is not found.
        // var addEditLocationWindow = new AddEditLocationWindow();
        // if (place is not null) addEditLocationWindow.SetPlace(place, false);
        //
        // var result = addEditLocationWindow.ShowDialog();
        // if (result is not true) return;
        //
        // // ReSharper disable once HeapView.DelegateAllocation
        // var oldPlace = PlacesCollection.FirstOrDefault(s => s.Id == History.PlaceFk);
        // if (addEditLocationWindow.PlaceDeleted)
        // {
        //     if (oldPlace is not null) PlacesCollection.Remove(oldPlace);
        //
        //     return;
        // }
        //
        // var editedPlace = addEditLocationWindow.Place;
        // Log.Information("Attempting to update place id:\"{EditedPlaceId}\", name:\"{EditedPlaceName}\"", editedPlace.Id,
        //     editedPlace.Name);
        //
        // var (success, exception) = editedPlace.AddOrEdit();
        // if (success)
        // {
        //     PlacesCollection!.AddAndSort(oldPlace, editedPlace, s => s!.Name!);
        //     History.PlaceFk = editedPlace.Id;
        //
        //     Log.Information("Place was successfully edited");
        //
        //     // Loop crash
        //     // var json = editedPlace.ToJsonString();
        //     // Log.Information("{Json}", json);
        //
        //     MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceSuccessMessage, MsgBoxImage.Check);
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     MsgBox.Show(LocationManagementResources.MessageBoxEditPlaceErrorMessage, MsgBoxImage.Error);
        // }
    }

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // The serviceProvider and items are set to null because they are not required in this context.
        // The ValidationResults list will store any validation errors detected during the process.
        var validationContext = new ValidationContext(History, serviceProvider: null, items: null);

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // is evident from the initialization. The result will still be compatible with any method
        // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(History, validationContext, validationResults, true);

        if (!isValid)
        {
            var propertyError = validationResults.First();
            var propertyMemberName = propertyError.MemberNames.First();

            var messageErrorKey = propertyMemberName switch
            {
                nameof(THistory.AccountFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationAccountFkError),
                nameof(THistory.Description) => nameof(DetailedRecordManagementResources.MessageBoxValidationDescriptionError),
                nameof(THistory.CategoryTypeFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationCategoryTypeFkError),
                nameof(THistory.ModePaymentFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationModePaymentFkError),
                nameof(THistory.Value) => nameof(DetailedRecordManagementResources.MessageBoxValidationValueError),
                nameof(THistory.Date) => nameof(DetailedRecordManagementResources.MessageBoxValidationDateError),
                nameof(THistory.PlaceFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationPlaceFkError),
                _ => null
            };

            var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
                ? propertyError.ErrorMessage!
                : DetailedRecordManagementResources.ResourceManager.GetString(messageErrorKey)!;

            MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
            return;
        }

        Log.Information("Attempting to inject the new history");

        History.DateAdded = DateTime.Now;
        if (History.IsPointed) History.DatePointed = DateTime.Now;
        else History.DatePointed = null;

        var (success, exception) = History.AddOrEdit();
        if (success)
        {
            Log.Information("History was successfully added");
            var json = History.ToJsonString();
            Log.Information("{Json}", json);

            MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistorySuccessMessage, MsgBoxImage.Check);

            // if (EditHistory)
            // {
            //     if (History.BankTransferFk is not null)
            //     {
            //         using var context = new DataBaseContextOld();
            //         var bankTransfer = context.TBankTransfers.FirstOrDefault(s => s.Id == History.BankTransferFk);
            //         if (bankTransfer is not null)
            //         {
            //             bankTransfer.MainReason = History.Description;
            //             bankTransfer.Value = Math.Abs(History.Value ?? 0);
            //             bankTransfer.AddOrEdit();
            //         }
            //     }
            //
            //     nameof(MainWindow.FrameBody).GoBack();
            //     return;
            // }

            var response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistoryQuestionMessage, MsgBoxImage.Question,
                MessageBoxButton.YesNoCancel);
            if (response is not MessageBoxResult.Yes) nameof(MainWindow.FrameBody).GoBack();

            History.Reset();
        }
        else
        {
            Log.Error(exception, "An error occurred please retry");
            MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistoryErrorMessage, MsgBoxImage.Error);
        }
    }

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => nameof(MainWindow.FrameBody).GoBack();

    private void ButtonDateNow_OnClick(object sender, RoutedEventArgs e)
        => History.Date = DateTime.Now;

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        var response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionMessage, MsgBoxImage.Question,
            MessageBoxButton.YesNoCancel);

        if (response is not MessageBoxResult.Yes) return;

        Log.Information("Attempting to remove the record \"{HistoryToDeleteDescriiption}\"", History.Description);
        var (success, exception) = History.Delete();

        if (success)
        {
            Log.Information("This Record was successfully removed");
            MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryNoUseSuccessMessage, MsgBoxImage.Check);

            DeleteBankTransfer();

            nameof(MainWindow.FrameBody).GoBack();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteUseRecordQuestionMessage,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove this record \"{HistoryToDeleteDescriiption}\" with all relative element",
                History.Description);
            History.Delete(true);
            DeleteBankTransfer();
            Log.Information("This record and all relative element was successfully removed");

            MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryUseSuccessMessage, MsgBoxImage.Check);

            nameof(MainWindow.FrameBody).GoBack();
            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryErrorMessage, MsgBoxImage.Error);
    }

    private void Configuration_OnConfigurationChanged()
        => UpdateConfiguration();

    #endregion

    #region Function

    private void DeleteBankTransfer()
    {
        if (History.BankTransferFk is null) return;

        var bankTransfer = History.BankTransferFk?.ToISql<TBankTransfer>();
        bankTransfer?.Delete(true);
    }

    public void SetTHistory(THistory history)
    {
        var place = history.PlaceFk?.ToISql<TPlace>();
        // SelectedCountry = EmptyStringTreeViewConverter.ToUnknown(place?.Country);
        // SelectedCity = EmptyStringTreeViewConverter.ToUnknown(place?.City);

        history.CopyPropertiesTo(History);
        // EditHistory = true;
    }

    private void UpdateConfiguration()
    {
        var configuration = Config.Configuration;
        TimePicker.Is24Hours = configuration.Interface.Clock.Is24Hours;

        var backColor = Utils.Resources.GetMaterialDesignPaperMapsUiStylesColor();
        MapControl.Map.BackColor = backColor;
    }

    #endregion

    private void MapControl_OnContextMenuOpening(object sender, ContextMenuEventArgs e)
    {
        var position = Mouse.GetPosition(MapControl);
        ExpenseManagementViewModel.LocationManagementViewModel.OnPositionChanged(position.X, position.Y, MapControl, true);
    }
}
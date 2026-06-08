using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModel;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Properties;
using MyExpenses.SharedUtils.Resources.Resx.DetailedRecordManagement;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MyExpenses.Wpf.Pages;

public partial class RecordExpensePage : IReceiveNavigationParameter
{
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public THistory History { get; } = new();

    private ExpenseManagementViewModel ExpenseManagementViewModel
        => (ExpenseManagementViewModel)DataContext;

    public RecordExpensePage(ExpenseManagementViewModel vm)
    {
        InitializeComponent();

        UpdateConfiguration();

        // ReSharper disable once HeapView.DelegateAllocation
        Configuration.ConfigurationChanged += Configuration_OnConfigurationChanged;

        DataContext = vm;
    }

    #region Action

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // The serviceProvider and items are set to null because they are not required in this context.
        // // The ValidationResults list will store any validation errors detected during the process.
        // var validationContext = new ValidationContext(History, serviceProvider: null, items: null);
        //
        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // Using 'var' keeps the code concise and readable, as the type (List<ValidationResult>)
        // // is evident from the initialization. The result will still be compatible with any method
        // // that expects an ICollection<ValidationResult>, as List<T> implements the ICollection interface.
        // var validationResults = new List<ValidationResult>();
        // var isValid = Validator.TryValidateObject(History, validationContext, validationResults, true);
        //
        // if (!isValid)
        // {
        //     var propertyError = validationResults.First();
        //     var propertyMemberName = propertyError.MemberNames.First();
        //
        //     var messageErrorKey = propertyMemberName switch
        //     {
        //         nameof(THistory.AccountFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationAccountFkError),
        //         nameof(THistory.Description) => nameof(DetailedRecordManagementResources.MessageBoxValidationDescriptionError),
        //         nameof(THistory.CategoryTypeFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationCategoryTypeFkError),
        //         nameof(THistory.ModePaymentFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationModePaymentFkError),
        //         nameof(THistory.Value) => nameof(DetailedRecordManagementResources.MessageBoxValidationValueError),
        //         nameof(THistory.Date) => nameof(DetailedRecordManagementResources.MessageBoxValidationDateError),
        //         nameof(THistory.PlaceFk) => nameof(DetailedRecordManagementResources.MessageBoxValidationPlaceFkError),
        //         _ => null
        //     };
        //
        //     var localizedErrorMessage = string.IsNullOrEmpty(messageErrorKey)
        //         ? propertyError.ErrorMessage!
        //         : DetailedRecordManagementResources.ResourceManager.GetString(messageErrorKey)!;
        //
        //     MsgBox.Show(localizedErrorMessage, MsgBoxImage.Error);
        //     return;
        // }
        //
        // Log.Information("Attempting to inject the new history");
        //
        // History.DateAdded = DateTime.Now;
        // if (History.IsPointed) History.DatePointed = DateTime.Now;
        // else History.DatePointed = null;
        //
        // var (success, exception) = History.AddOrEdit();
        // if (success)
        // {
        //     Log.Information("History was successfully added");
        //     var json = History.ToJsonString();
        //     Log.Information("{Json}", json);
        //
        //     MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistorySuccessMessage, MsgBoxImage.Check);
        //
        //     // if (EditHistory)
        //     // {
        //     //     if (History.BankTransferFk is not null)
        //     //     {
        //     //         using var context = new DataBaseContextOld();
        //     //         var bankTransfer = context.TBankTransfers.FirstOrDefault(s => s.Id == History.BankTransferFk);
        //     //         if (bankTransfer is not null)
        //     //         {
        //     //             bankTransfer.MainReason = History.Description;
        //     //             bankTransfer.Value = Math.Abs(History.Value ?? 0);
        //     //             bankTransfer.AddOrEdit();
        //     //         }
        //     //     }
        //     //
        //     //     nameof(MainWindow.FrameBody).GoBack();
        //     //     return;
        //     // }
        //
        //     var response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistoryQuestionMessage, MsgBoxImage.Question,
        //         MessageBoxButton.YesNoCancel);
        //     if (response is not MessageBoxResult.Yes) nameof(MainWindow.FrameBody).GoBack();
        //
        //     History.Reset();
        // }
        // else
        // {
        //     Log.Error(exception, "An error occurred please retry");
        //     MsgBox.Show(DetailedRecordManagementResources.MessageBoxAddHistoryErrorMessage, MsgBoxImage.Error);
        // }
    }

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

    /// <summary>
    /// Receives navigation parameters from the NavigationService.
    /// This is called when the page receives a navigation parameter.
    /// </summary>
    /// <param name="parameter">The parameter passed during navigation</param>
    public void OnNavigationParameterReceived(object? parameter)
    {
        if (parameter is not HistoryViewModel historyViewModel) return;

        ExpenseManagementViewModel.Load(historyViewModel);
    }
}
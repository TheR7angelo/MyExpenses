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

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {


        // var response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryQuestionMessage, MsgBoxImage.Question,
        //     MessageBoxButton.YesNoCancel);
        //
        // if (response is not MessageBoxResult.Yes) return;
        //
        // Log.Information("Attempting to remove the record \"{HistoryToDeleteDescriiption}\"", History.Description);
        // var (success, exception) = History.Delete();
        //
        // if (success)
        // {
        //     Log.Information("This Record was successfully removed");
        //     MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryNoUseSuccessMessage, MsgBoxImage.Check);
        //
        //     DeleteBankTransfer();
        //
        //     nameof(MainWindow.FrameBody).GoBack();
        //     return;
        // }
        //
        // if (exception!.InnerException is SqliteException
        //     {
        //         SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
        //     })
        // {
        //     Log.Error("Foreign key constraint violation");
        //
        //     response = MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteUseRecordQuestionMessage,
        //         MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        //
        //     if (response is not MessageBoxResult.Yes) return;
        //
        //     Log.Information(
        //         "Attempting to remove this record \"{HistoryToDeleteDescriiption}\" with all relative element",
        //         History.Description);
        //     History.Delete(true);
        //     DeleteBankTransfer();
        //     Log.Information("This record and all relative element was successfully removed");
        //
        //     MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryUseSuccessMessage, MsgBoxImage.Check);
        //
        //     nameof(MainWindow.FrameBody).GoBack();
        //     return;
        // }
        //
        // Log.Error(exception, "An error occurred please retry");
        // MsgBox.Show(DetailedRecordManagementResources.MessageBoxDeleteHistoryErrorMessage, MsgBoxImage.Error);
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
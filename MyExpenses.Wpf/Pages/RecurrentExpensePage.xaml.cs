using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Presentation.Enums;
using MyExpenses.Presentation.Services.Interfaces;
using MyExpenses.Presentation.ViewModels.Expenses;
using MyExpenses.SharedUtils.Collection;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Resources.Resx.Pages.RecurrentExpensePage;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.Dialogs.MsgBox;
using Serilog;
using MessageBoxButton = System.Windows.MessageBoxButton;
using MessageBoxResult = System.Windows.MessageBoxResult;

namespace MyExpenses.Wpf.Pages;

public partial class RecurrentExpensePage
{
    private DataGridRow? DataGridRow { get; set; }

    public ObservableCollection<RecursiveExpenseViewModel> RecurringExpenses { get; } = [];

    private readonly INavigationWindowService _navigationWindowService;

    public RecurrentExpensePage(INavigationWindowService navigationWindowService)
    {
        _navigationWindowService = navigationWindowService;

        InitializeComponent();

        UpdateDataGrid();
    }

    #region Action

    private void ButtonAddNewRecurrent_OnClick(object sender, RoutedEventArgs e)
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // An instance of AddEditRecurrentExpenseWindow is created to manage adding or editing recurrent expenses.
        // ShowDialog() is called to display the window modally and capture the user's action.
        // If the dialog result is not true (e.g., the user cancels the operation), the method returns early.
        // var addEditRecurrentExpenseWindow = new AddEditRecurrentExpenseWindow();
        //
        // var result = addEditRecurrentExpenseWindow.ShowDialog();
        // if (result is not true) return;
        //
        // UpdateDataGrid();

        // TODO correct
        _navigationWindowService.ShowManageRecurringExpense();
    }

    private void ButtonDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VRecursiveExpense vRecurrentExpense) return;

        DeleteRecurrentExpense(vRecurrentExpense);
    }

    private void ButtonEditRecord_OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        if (button.DataContext is not VRecursiveExpense vRecurrentExpense) return;

        EditRecurrentExpense(vRecurrentExpense);
    }

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void MenuItemDeleteRecord_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow?.DataContext is not VRecursiveExpense vRecurrentExpense) return;

        DeleteRecurrentExpense(vRecurrentExpense);
    }

    private void MenuItemEditRecurrentExpense_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow?.DataContext is not VRecursiveExpense vRecurrentExpense) return;

        EditRecurrentExpense(vRecurrentExpense);
    }

    #endregion

    #region Function

    private void DeleteRecurrentExpense(VRecursiveExpense vRecurrentExpense)
    {
        var response = MsgBox.Show(RecurrentExpensePageResources.MessageBoxDeleteQuestion,
            MsgBoxImage.Question, MessageBoxButton.YesNoCancel);
        if (response is not MessageBoxResult.Yes) return;

        var recurrentExpense = vRecurrentExpense.Id.ToISql<TRecursiveExpense>()!;
        Log.Information("Attempting to remove the recursive expense \"{RecursiveExpenseDescription}\"",
            recurrentExpense.Description);
        var (success, exception) = recurrentExpense.Delete();

        if (success)
        {
            Log.Information("Recursive expense was successfully removed");
            MsgBox.Show(RecurrentExpensePageResources.MessageBoxDeleteRecursiveExpenseNoUseSuccess,
                MsgBoxImage.Check);

            UpdateDataGrid();
            return;
        }

        if (exception!.InnerException is SqliteException
            {
                SqliteExtendedErrorCode: SQLitePCL.raw.SQLITE_CONSTRAINT_FOREIGNKEY
            })
        {
            Log.Error("Foreign key constraint violation");

            response = MsgBox.Show(RecurrentExpensePageResources.MessageBoxDeleteRecursiveExpenseUseQuestion,
                MsgBoxImage.Question, MessageBoxButton.YesNoCancel);

            if (response is not MessageBoxResult.Yes) return;

            Log.Information(
                "Attempting to remove the recursive expense \"{RecursiveExpenseDescription}\" with all relative element",
                recurrentExpense.Description);
            recurrentExpense.Delete(true);
            Log.Information("Recursive expense and all relative element was successfully removed");
            MsgBox.Show(RecurrentExpensePageResources.MessageBoxDeleteRecursiveExpenseUseSuccess,
                MsgBoxImage.Check);

            UpdateDataGrid();
            return;
        }

        Log.Error(exception, "An error occurred please retry");
        MsgBox.Show(RecurrentExpensePageResources.MessageBoxDeleteRecursiveExpenseError, MsgBoxImage.Error);
    }

    private void EditRecurrentExpense(VRecursiveExpense vRecurrentExpense)
    {
        // TODO correct

        // // ReSharper disable once HeapView.ObjectAllocation.Evident
        // // An instance of AddEditRecurrentExpenseWindow is created to manage adding or editing recurrent expenses.
        // // ShowDialog() is called to display the window modally and capture the user's action.
        // // If the dialog result is not true (e.g., the user cancels the operation), the method returns early.
        // var addEditRecurrentExpenseWindow = new AddEditRecurrentExpenseWindow();
        // addEditRecurrentExpenseWindow.SetVRecursiveExpense(vRecurrentExpense);
        //
        // var result = addEditRecurrentExpenseWindow.ShowDialog();
        // if (result is not true) return;
        //
        // UpdateDataGrid();
    }

    private async void UpdateDataGrid()
    {
        // TODO correct
        // var result = await _expensePresentationService.GetAllRecurringExpense();
        // if (result.IsSuccess)
        // {
        //     var records = result.Value!;
        //
        //     RecurringExpenses.Clear();
        //     RecurringExpenses.AddRange(records);
        // }
        // else
        // {
        //     MessageBox.Show(result.ErrorCode.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        // }
    }

    #endregion
}
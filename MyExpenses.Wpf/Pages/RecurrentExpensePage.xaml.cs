using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilterDataGrid;
using Microsoft.Data.Sqlite;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Resx.Pages.RecurrentExpensePage;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using MyExpenses.Wpf.Windows;
using MyExpenses.Wpf.Windows.MsgBox;
using Serilog;

namespace MyExpenses.Wpf.Pages;

public partial class RecurrentExpensePage
{
    #region DependencyProperty

    // ReSharper disable once HeapView.BoxingAllocation
    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(RecurrentExpensePage), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
        // ReSharper disable once HeapView.BoxingAllocation
        set => SetValue(LocalLanguageProperty, value);
    }

    public static readonly DependencyProperty DateFormatStringProperty =
        DependencyProperty.Register(nameof(DateFormatString), typeof(string), typeof(RecurrentExpensePage),
            new PropertyMetadata(default(string)));

    public string DateFormatString
    {
        get => (string)GetValue(DateFormatStringProperty);
        set => SetValue(DateFormatStringProperty, value);
    }

    public static readonly DependencyProperty DataGridMenuItemHeaderEditRecordProperty =
        DependencyProperty.Register(nameof(DataGridMenuItemHeaderEditRecord), typeof(string),
            typeof(RecurrentExpensePage), new PropertyMetadata(default(string)));

    public string DataGridMenuItemHeaderEditRecord
    {
        get => (string)GetValue(DataGridMenuItemHeaderEditRecordProperty);
        set => SetValue(DataGridMenuItemHeaderEditRecordProperty, value);
    }

    public static readonly DependencyProperty DataGridMenuItemHeaderDeleteRecordProperty =
        DependencyProperty.Register(nameof(DataGridMenuItemHeaderDeleteRecord), typeof(string),
            typeof(RecurrentExpensePage), new PropertyMetadata(default(string)));

    public string DataGridMenuItemHeaderDeleteRecord
    {
        get => (string)GetValue(DataGridMenuItemHeaderDeleteRecordProperty);
        set => SetValue(DataGridMenuItemHeaderDeleteRecordProperty, value);
    }

    public static readonly DependencyProperty ButtonContentEditRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentEditRecord), typeof(string), typeof(RecurrentExpensePage),
            new PropertyMetadata(default(string)));

    public string ButtonContentEditRecord
    {
        get => (string)GetValue(ButtonContentEditRecordProperty);
        set => SetValue(ButtonContentEditRecordProperty, value);
    }

    public static readonly DependencyProperty ButtonContentDeleteRecordProperty =
        DependencyProperty.Register(nameof(ButtonContentDeleteRecord), typeof(string), typeof(RecurrentExpensePage),
            new PropertyMetadata(default(string)));

    public string ButtonContentDeleteRecord
    {
        get => (string)GetValue(ButtonContentDeleteRecordProperty);
        set => SetValue(ButtonContentDeleteRecordProperty, value);
    }

    #endregion

    private DataGridRow? DataGridRow { get; set; }

    public ObservableCollection<VRecursiveExpense> VRecursiveExpenses { get; } = [];

    public RecurrentExpensePage()
    {
        UpdateLocalLanguage();

        InitializeComponent();

        UpdateLanguage();

        UpdateDataGrid();

        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void ButtonAddNewRecurrent_OnClick(object sender, RoutedEventArgs e)
    {
        var addEditRecurrentExpenseWindow = new AddEditRecurrentExpenseWindow();

        var result = addEditRecurrentExpenseWindow.ShowDialog();
        if (result is not true) return;

        UpdateDataGrid();
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

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

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
        var addEditRecurrentExpenseWindow = new AddEditRecurrentExpenseWindow();
        addEditRecurrentExpenseWindow.SetVRecursiveExpense(vRecurrentExpense);

        var result = addEditRecurrentExpenseWindow.ShowDialog();
        if (result is not true) return;

        UpdateDataGrid();
    }

    private void UpdateDataGrid()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        // Necessary instantiation of DataBaseContext to interact with the database.
        // This creates a scoped database context for performing queries and modifications in the database.
        using var context = new DataBaseContext();
        var records = context.VRecursiveExpenses
            .OrderBy(s => !s.ForceDeactivate)
            .ThenBy(s => !s.IsActive)
            .ThenBy(s => s.NextDueDate)
            .ToList();

        VRecursiveExpenses.Clear();
        VRecursiveExpenses.AddRange(records);
    }

    private void UpdateLanguage()
    {
        DateFormatString = RecurrentExpensePageResources.DateFormatString;
        UpdateLocalLanguage();

        DataGridTextColumnAccount.Header = RecurrentExpensePageResources.DataGridTextColumnAccountHeader;
        DataGridTextColumnDescription.Header = RecurrentExpensePageResources.DataGridTextColumnDescriptionHeader;
        DataGridTextColumnNote.Header = RecurrentExpensePageResources.DataGridTextColumnNoteHeader;
        DataGridTemplateColumnCategory.Header = RecurrentExpensePageResources.DataGridTemplateColumnCategoryHeader;
        DataGridTextColumnModePayment.Header = RecurrentExpensePageResources.DataGridTextColumnModePaymentHeader;
        DataGridTemplateColumnValue.Header = RecurrentExpensePageResources.DataGridTemplateColumnValueHeader;
        DataGridTextColumnStartDate.Header = RecurrentExpensePageResources.DataGridTextColumnStartDateHeader;
        DataGridTextColumnRecursiveTotal.Header = RecurrentExpensePageResources.DataGridTextColumnRecursiveTotalHeader;
        DataGridTextColumnRecursiveCount.Header = RecurrentExpensePageResources.DataGridTextColumnRecursiveCountHeader;
        DataGridTextColumnFrequency.Header = RecurrentExpensePageResources.DataGridTextColumnFrequencyHeader;
        DataGridTextColumnNextDueDate.Header = RecurrentExpensePageResources.DataGridTextColumnNextDueDateHeader;
        DataGridTextColumnPlace.Header = RecurrentExpensePageResources.DataGridTextColumnPlaceHeader;
        DataGridCheckBoxColumnIsActive.Header = RecurrentExpensePageResources.DataGridCheckBoxColumnIsActiveHeader;
        DataGridCheckBoxColumnForceDeactivate.Header = RecurrentExpensePageResources.DataGridCheckBoxColumnForceDeactivateHeader;
        DataGridTemplateColumnActions.Header = RecurrentExpensePageResources.DataGridTemplateColumnActionsHeader;
        DataGridMenuItemHeaderEditRecord = RecurrentExpensePageResources.DataGridMenuItemHeaderEditRecord;
        DataGridMenuItemHeaderDeleteRecord = RecurrentExpensePageResources.DataGridMenuItemHeaderDeleteRecord;

        ButtonContentEditRecord = RecurrentExpensePageResources.ButtonContentEditRecord;
        ButtonContentDeleteRecord = RecurrentExpensePageResources.ButtonContentDeleteRecord;
    }

    private void UpdateLocalLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();
    }

    #endregion
}
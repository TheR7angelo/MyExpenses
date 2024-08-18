using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilterDataGrid;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Resx.Pages.RecurrentExpensePage;
using MyExpenses.Wpf.Utils.FilterDataGrid;
using MyExpenses.Wpf.Windows;

namespace MyExpenses.Wpf.Pages;

public partial class RecurrentExpensePage
{
    #region DependencyProperty

    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(RecurrentExpensePage), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
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

    #endregion

    private DataGridRow? DataGridRow { get; set; }

    public ObservableCollection<VRecursiveExpense> VRecursiveExpenses { get; }

    public RecurrentExpensePage()
    {
        UpdateLocalLanguage();

        InitializeComponent();

        UpdateLanguage();
        VRecursiveExpenses = [];
        FilterDataGrid.ItemsSource = VRecursiveExpenses;

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

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void MenuItemEditRecurrentExpense_OnClick(object sender, RoutedEventArgs e)
    {
        if (DataGridRow?.DataContext is not VRecursiveExpense vRecurrentExpense) return;

        EditRecurrentExpenseWindow(vRecurrentExpense);
    }

    #endregion

    #region Function

    private void EditRecurrentExpenseWindow(VRecursiveExpense vRecurrentExpense)
    {
        var addEditRecurrentExpenseWindow = new AddEditRecurrentExpenseWindow();
        addEditRecurrentExpenseWindow.SetVRecursiveExpense(vRecurrentExpense);

        var result = addEditRecurrentExpenseWindow.ShowDialog();
        if (result is not true) return;

        UpdateDataGrid();
    }

    private void UpdateDataGrid()
    {
        using var context = new DataBaseContext();
        var records = context.VRecursiveExpenses
            .OrderBy(s => !s.ForceDeactivate)
            .ThenBy(s => s.IsActive)
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
        DataGridMenuItemHeaderEditRecord = RecurrentExpensePageResources.DataGridMenuItemHeaderEditRecord;
    }

    private void UpdateLocalLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();
    }

    #endregion
}
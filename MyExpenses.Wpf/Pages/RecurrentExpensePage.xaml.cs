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

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateDataGrid()
    {
        using var context = new DataBaseContext();
        var records = context.VRecursiveExpenses
            .OrderBy(s => s.IsActive)
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
        DataGridCheckBoxColumnForceDeactivate.Header = "Force deactivate";
    }

    private void UpdateLocalLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();
    }

    #endregion

    private void ButtonAddNewRecurent_OnClick(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("Adding new Recurent Expense");
    }
}
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FilterDataGrid;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Enums;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Wpf.Resources.Resx.Windows.RecurrentAddWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;

namespace MyExpenses.Wpf.Windows;

public partial class RecurrentAddWindow
{
    #region Properties

    #region DependencyProperty

    #region DataGrid

    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(RecurrentAddWindow), new PropertyMetadata(default(Local)));

    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
        set => SetValue(LocalLanguageProperty, value);
    }

    public static readonly DependencyProperty DateFormatStringProperty =
        DependencyProperty.Register(nameof(DateFormatString), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string DateFormatString
    {
        get => (string)GetValue(DateFormatStringProperty);
        set => SetValue(DateFormatStringProperty, value);
    }

    #endregion

    #region Button

    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    #endregion

    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    public static readonly DependencyProperty TextBlockAddRecurrenceNeededProperty =
        DependencyProperty.Register(nameof(TextBlockAddRecurrenceNeeded), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string TextBlockAddRecurrenceNeeded
    {
        get => (string)GetValue(TextBlockAddRecurrenceNeededProperty);
        set => SetValue(TextBlockAddRecurrenceNeededProperty, value);
    }

    #endregion

    #endregion

    private DataGridRow? DataGridRow { get; set; }

    public ObservableCollection<VRecursiveExpenseDerive> VRecursiveExpensesDerives { get; }

    public RecurrentAddWindow(double currentWidth)
    {
        UpdateLocalLanguage();

        InitializeComponent();

        UpdateLanguage();
        Width = currentWidth;

        VRecursiveExpensesDerives = [];
        FilterDataGrid.ItemsSource = VRecursiveExpensesDerives;

        UpdateDataGrid();

        Interface.LanguageChanged += Interface_OnLanguageChanged;

        this.SetWindowCornerPreference();
    }

    #region Action

    private void ButtonCancel_OnClick(object sender, RoutedEventArgs e)
        => Close();

    private void ButtonValid_OnClick(object sender, RoutedEventArgs e)
    {
        var vRecursiveExpenseDerives = VRecursiveExpensesDerives
            .Where(s => s.RecursiveToAdd);

        var timeOnly = new TimeOnly(0);
        foreach (var vRecursiveExpenseDerive in vRecursiveExpenseDerives)
        {
            var history = new THistory
            {
                AccountFk = vRecursiveExpenseDerive.AccountFk,
                Description = vRecursiveExpenseDerive.Description,
                CategoryTypeFk = vRecursiveExpenseDerive.CategoryTypeFk,
                ModePaymentFk = vRecursiveExpenseDerive.ModePaymentFk,
                Value = vRecursiveExpenseDerive.Value,
                Date = vRecursiveExpenseDerive.NextDueDate.ToDateTime(timeOnly),
                PlaceFk = vRecursiveExpenseDerive.PlaceFk,
                RecursiveExpenseFk = vRecursiveExpenseDerive.Id
            };
            history.AddOrEdit();

            var recurcive = vRecursiveExpenseDerive.Id.ToISql<TRecursiveExpense>()!;
            recurcive = UpdateTRecursiveExpense(recurcive);
            recurcive.AddOrEdit();
        }

        Close();
    }

    private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        => DataGridRow = sender as DataGridRow;

    private TRecursiveExpense UpdateTRecursiveExpense(TRecursiveExpense recursiveExpense)
    {
        recursiveExpense.RecursiveCount += 1;
        recursiveExpense.LastUpdated = DateTime.Now;

        if (recursiveExpense.RecursiveTotal.HasValue &&
            recursiveExpense.RecursiveTotal < recursiveExpense.RecursiveCount)
        {
            return recursiveExpense;
        }

        recursiveExpense.NextDueDate = recursiveExpense.ERecursiveFrequency switch
        {
            ERecursiveFrequency.Daily => recursiveExpense.NextDueDate.AddDays(1),
            ERecursiveFrequency.Weekly => recursiveExpense.NextDueDate.AddDays(7),
            ERecursiveFrequency.Monthly => recursiveExpense.NextDueDate.AddMonths(1),
            ERecursiveFrequency.Yearly => recursiveExpense.NextDueDate.AddYears(1),
            _ => throw new ArgumentOutOfRangeException()
        };

        return recursiveExpense;
    }

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    #endregion

    #region Function

    private void UpdateDataGrid()
    {
        var mapper = Mapping.Mapper;

        var now = DateTime.Now;
        using var context = new DataBaseContext();
        var records = context.TRecursiveExpenses
            .Where(s => !s.ForceDeactivate)
            .Where(s => s.IsActive)
            .Where(s => s.NextDueDate.Year.Equals(now.Year) && s.NextDueDate.Month.Equals(now.Month))
            .OrderBy(s => s.NextDueDate)
            .Select(s => s.Id.ToISql<VRecursiveExpense>())!
            .Select(s => mapper.Map<VRecursiveExpenseDerive>(s));

        VRecursiveExpensesDerives.Clear();
        VRecursiveExpensesDerives.AddRange(records);
    }

    private void UpdateLanguage()
    {
        TitleWindow = RecurrentAddWindowResources.TitleWindow;
        TextBlockAddRecurrenceNeeded = RecurrentAddWindowResources.TextBlockAddRecurrenceNeeded;

        UpdateLocalLanguage();
        DateFormatString = RecurrentAddWindowResources.DataGridDateFormatString;

        DataGridTextColumnAccount.Header = RecurrentAddWindowResources.DataGridTextColumnAccountHeader;
        DataGridTextColumnDescription.Header = RecurrentAddWindowResources.DataGridTextColumnDescriptionHeader;
        DataGridTextColumnNote.Header = RecurrentAddWindowResources.DataGridTextColumnNoteHeader;
        DataGridTemplateColumnCategory.Header = RecurrentAddWindowResources.DataGridTemplateColumnCategoryHeader;
        DataGridTextColumnModePayment.Header = RecurrentAddWindowResources.DataGridTextColumnModePaymentHeader;
        DataGridTemplateColumnValue.Header = RecurrentAddWindowResources.DataGridTemplateColumnValueHeader;
        DataGridTextColumnStartDate.Header = RecurrentAddWindowResources.DataGridTextColumnStartDateHeader;
        DataGridTextColumnRecursiveTotal.Header = RecurrentAddWindowResources.DataGridTextColumnRecursiveTotalHeader;
        DataGridTextColumnRecursiveCount.Header = RecurrentAddWindowResources.DataGridTextColumnRecursiveCountHeader;
        DataGridTextColumnFrequency.Header = RecurrentAddWindowResources.DataGridTextColumnFrequencyHeader;
        DataGridTextColumnNextDueDate.Header = RecurrentAddWindowResources.DataGridTextColumnNextDueDateHeader;
        DataGridTextColumnPlace.Header = RecurrentAddWindowResources.DataGridTextColumnPlaceHeader;
        DataGridCheckBoxColumnRecursiveToAdd.Header = RecurrentAddWindowResources.DataGridCheckBoxColumnRecursiveToAddHeader;

        ButtonValidContent = RecurrentAddWindowResources.ButtonValidContent;
        ButtonCancelContent = RecurrentAddWindowResources.ButtonCancelContent;
    }

    private void UpdateLocalLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;
        LocalLanguage = currentCulture.ToLocal();
    }

    #endregion
}
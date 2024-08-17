using System.Windows;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Enums;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.Windows;

public partial class RecurrentAddWindow
{
    #region Properties

    #region DependencyProperty

    #region DataGrid

    public static readonly DependencyProperty DataGridTextColumnAccountHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTextColumnAccountHeader), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string DataGridTextColumnAccountHeader
    {
        get => (string)GetValue(DataGridTextColumnAccountHeaderProperty);
        set => SetValue(DataGridTextColumnAccountHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridTextColumnDescriptionHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTextColumnDescriptionHeader), typeof(string),
            typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string DataGridTextColumnDescriptionHeader
    {
        get => (string)GetValue(DataGridTextColumnDescriptionHeaderProperty);
        set => SetValue(DataGridTextColumnDescriptionHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridTemplateColumnCategoryHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTemplateColumnCategoryHeader), typeof(string),
            typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string DataGridTemplateColumnCategoryHeader
    {
        get => (string)GetValue(DataGridTemplateColumnCategoryHeaderProperty);
        set => SetValue(DataGridTemplateColumnCategoryHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridTextColumnModePaymentHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTextColumnModePaymentHeader), typeof(string),
            typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string DataGridTextColumnModePaymentHeader
    {
        get => (string)GetValue(DataGridTextColumnModePaymentHeaderProperty);
        set => SetValue(DataGridTextColumnModePaymentHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridTemplateColumnValueHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTemplateColumnValueHeader), typeof(string),
            typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string DataGridTemplateColumnValueHeader
    {
        get => (string)GetValue(DataGridTemplateColumnValueHeaderProperty);
        set => SetValue(DataGridTemplateColumnValueHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridTextColumnNextDueDateHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTextColumnNextDueDateHeader), typeof(string),
            typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string DataGridTextColumnNextDueDateHeader
    {
        get => (string)GetValue(DataGridTextColumnNextDueDateHeaderProperty);
        set => SetValue(DataGridTextColumnNextDueDateHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridTextColumnPlaceHeaderProperty =
        DependencyProperty.Register(nameof(DataGridTextColumnPlaceHeader), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string DataGridTextColumnPlaceHeader
    {
        get => (string)GetValue(DataGridTextColumnPlaceHeaderProperty);
        set => SetValue(DataGridTextColumnPlaceHeaderProperty, value);
    }

    public static readonly DependencyProperty DataGridCheckBoxColumnRecursiveToAddHeaderProperty =
        DependencyProperty.Register(nameof(DataGridCheckBoxColumnRecursiveToAddHeader), typeof(string),
            typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string DataGridCheckBoxColumnRecursiveToAddHeader
    {
        get => (string)GetValue(DataGridCheckBoxColumnRecursiveToAddHeaderProperty);
        set => SetValue(DataGridCheckBoxColumnRecursiveToAddHeaderProperty, value);
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

    #endregion

    public string TextBlockAddRecurrenceNeeded
    {
        get => (string)GetValue(TextBlockAddRecurrenceNeededProperty);
        set => SetValue(TextBlockAddRecurrenceNeededProperty, value);
    }

    #endregion

    public List<VRecursiveExpenseDerive> VRecursiveExpensesDerives { get; }

    public RecurrentAddWindow(IEnumerable<TRecursiveExpense> recursiveExpenses, double currentWidth)
    {
        var mapper = Mapping.Mapper;
        VRecursiveExpensesDerives =
        [
            ..recursiveExpenses
                .Select(s => s.Id.ToISql<VRecursiveExpense>())!
                .Select(s => mapper.Map<VRecursiveExpenseDerive>(s))
        ];

        UpdateLanguage();
        InitializeComponent();
        Width = currentWidth;

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

    private void UpdateLanguage()
    {
        // TODO work
        TitleWindow = "RecurrentAddWindow";
        TextBlockAddRecurrenceNeeded = "Need to insert new recurrences";

        DataGridTextColumnAccountHeader = "DataGridTextColumnAccountHeader";
        DataGridTextColumnDescriptionHeader = "DataGridTextColumnDescriptionHeader";
        DataGridTemplateColumnCategoryHeader = "DataGridTemplateColumnCategoryHeader";
        DataGridTextColumnModePaymentHeader = "DataGridTextColumnModePaymentHeader";
        DataGridTemplateColumnValueHeader = "DataGridTemplateColumnValueHeader";
        DataGridTextColumnNextDueDateHeader = "DataGridTextColumnNextDueDateHeader";
        DataGridTextColumnPlaceHeader = "DataGridTextColumnPlaceHeader";
        DataGridCheckBoxColumnRecursiveToAddHeader = "DataGridCheckBoxColumnRecursiveToAddHeader";
    }

    #endregion
}
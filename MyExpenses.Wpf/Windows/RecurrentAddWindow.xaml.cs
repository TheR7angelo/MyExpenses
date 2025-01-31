using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using FilterDataGrid;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Tables;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Sql.Derivatives.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.DateTimes;
using MyExpenses.Wpf.Resources.Resx.Windows.RecurrentAddWindow;
using MyExpenses.Wpf.Utils;
using MyExpenses.Wpf.Utils.FilterDataGrid;

namespace MyExpenses.Wpf.Windows;

public partial class RecurrentAddWindow
{
    #region Properties

    #region DependencyProperty

    #region DataGrid

    // ReSharper disable once HeapView.BoxingAllocation
    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty LocalLanguageProperty = DependencyProperty.Register(nameof(LocalLanguage),
        typeof(Local), typeof(RecurrentAddWindow), new PropertyMetadata(default(Local)));

    // ReSharper disable once HeapView.BoxingAllocation
    public Local LocalLanguage
    {
        get => (Local)GetValue(LocalLanguageProperty);
        set => SetValue(LocalLanguageProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonCancelContentProperty =
        DependencyProperty.Register(nameof(ButtonCancelContent), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string ButtonCancelContent
    {
        get => (string)GetValue(ButtonCancelContentProperty);
        set => SetValue(ButtonCancelContentProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty ButtonValidContentProperty =
        DependencyProperty.Register(nameof(ButtonValidContent), typeof(string), typeof(RecurrentAddWindow),
            new PropertyMetadata(default(string)));

    public string ButtonValidContent
    {
        get => (string)GetValue(ButtonValidContentProperty);
        set => SetValue(ButtonValidContentProperty, value);
    }

    #endregion

    // ReSharper disable once HeapView.ObjectAllocation.Evident
    public static readonly DependencyProperty TitleWindowProperty = DependencyProperty.Register(nameof(TitleWindow),
        typeof(string), typeof(RecurrentAddWindow), new PropertyMetadata(default(string)));

    public string TitleWindow
    {
        get => (string)GetValue(TitleWindowProperty);
        set => SetValue(TitleWindowProperty, value);
    }

    // ReSharper disable once HeapView.ObjectAllocation.Evident
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

    // private DataGridRow? DataGridRow { get; set; }

    public ObservableCollection<VRecursiveExpenseDerive> VRecursiveExpensesDerives { get; } = [];

    public RecurrentAddWindow(Size currentSize)
    {
        UpdateLocalLanguage();

        InitializeComponent();

        UpdateLanguage();
        Width = currentSize.Width;
        Height = currentSize.Height;

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

        var mapper = Mapping.Mapper;
        foreach (var vRecursiveExpenseDerive in vRecursiveExpenseDerives)
        {
            var history = mapper.Map<THistory>(vRecursiveExpenseDerive);
            history.Date = DateTimeExtensions.ToDateTime(vRecursiveExpenseDerive.NextDueDate);
            history.AddOrEdit();

            var recursive = vRecursiveExpenseDerive.Id.ToISql<TRecursiveExpense>()!;
            recursive = UpdateTRecursiveExpense(recursive);
            recursive.AddOrEdit();
        }

        Close();
    }

    // private void DataGridRow_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    //     => DataGridRow = sender as DataGridRow;

    private static TRecursiveExpense UpdateTRecursiveExpense(TRecursiveExpense recursiveExpense)
    {
        recursiveExpense.RecursiveCount += 1;
        recursiveExpense.LastUpdated = DateTime.Now;

        if (recursiveExpense.RecursiveTotal.HasValue &&
            recursiveExpense.RecursiveTotal < recursiveExpense.RecursiveCount)
        {
            return recursiveExpense;
        }

        var dateOnly = recursiveExpense.ERecursiveFrequency.CalculateNextDueDate(recursiveExpense.NextDueDate);
        recursiveExpense.NextDueDate = dateOnly;

        return recursiveExpense;
    }

    private void Interface_OnLanguageChanged()
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
            .Select(s => s.Id.ToISql<VRecursiveExpense>())
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
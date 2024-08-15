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

        var histories = new List<THistory>();
        foreach (var vRecursiveExpenseDerive in vRecursiveExpenseDerives)
        {
            var history = new THistory
            {
                AccountFk = vRecursiveExpenseDerive.AccountFk,
                Description = vRecursiveExpenseDerive.Description,
                CategoryTypeFk = vRecursiveExpenseDerive.CategoryTypeFk,
                ModePaymentFk = vRecursiveExpenseDerive.ModePaymentFk,
                Value = vRecursiveExpenseDerive.Value,
                Date = vRecursiveExpenseDerive.NextDueDate,
                PlaceFk = vRecursiveExpenseDerive.PlaceFk,
                RecursiveExpenseFk = vRecursiveExpenseDerive.Id
            };
            histories.Add(history);

            var recurcive = vRecursiveExpenseDerive.Id.ToISql<TRecursiveExpense>()!;
            recurcive = UpdateTRecursiveExpense(recurcive);
            recurcive.AddOrEdit();
        }

        using var context = new DataBaseContext();
        context.THistories.AddRange(histories);
        context.SaveChanges();

        Close();
    }

    private TRecursiveExpense UpdateTRecursiveExpense(TRecursiveExpense recursiveExpense)
    {
        recursiveExpense.RecursiveCount += 1;

        if (recursiveExpense.RecursiveTotal.HasValue && recursiveExpense.RecursiveTotal < recursiveExpense.RecursiveCount)
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
    }

    #endregion
}
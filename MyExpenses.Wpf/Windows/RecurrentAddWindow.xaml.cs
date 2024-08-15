using System.Windows;
using MyExpenses.Models.AutoMapper;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
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
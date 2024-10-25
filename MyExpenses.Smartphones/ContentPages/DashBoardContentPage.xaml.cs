using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Globalization;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Utils.Collection;
using MyExpenses.Utils.Strings;

namespace MyExpenses.Smartphones.ContentPages;

public partial class DashBoardContentPage
{
    public static readonly BindableProperty CurrentVTotalByAccountProperty =
        BindableProperty.Create(nameof(CurrentVTotalByAccount), typeof(VTotalByAccount), typeof(DashBoardContentPage),
            default(VTotalByAccount));

    public VTotalByAccount? CurrentVTotalByAccount
    {
        get => (VTotalByAccount)GetValue(CurrentVTotalByAccountProperty);
        set => SetValue(CurrentVTotalByAccountProperty, value);
    }

    public static readonly BindableProperty SelectedYearProperty = BindableProperty.Create(nameof(SelectedYear),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string SelectedYear
    {
        get => (string)GetValue(SelectedYearProperty);
        set => SetValue(SelectedYearProperty, value);
    }

    public static readonly BindableProperty SelectedMonthProperty = BindableProperty.Create(nameof(SelectedMonth),
        typeof(string), typeof(DashBoardContentPage), default(string));

    public string SelectedMonth
    {
        get => (string)GetValue(SelectedMonthProperty);
        set => SetValue(SelectedMonthProperty, value);
    }

    public ObservableCollection<string> Years { get; }
    public ObservableCollection<string> Months { get; } = [];

    private static VTotalByAccount? _staticVTotalByAccount;
    private static VTotalByAccount? StaticVTotalByAccount
    {
        get => _staticVTotalByAccount;
        set
        {
            _staticVTotalByAccount = value;
            Instance.CurrentVTotalByAccount = value;
        }
    }

    private static DashBoardContentPage Instance { get; set; } = null!;

    public DashBoardContentPage()
    {
        Instance = this;

        UpdateMonthLanguage();

        var now = DateTime.Now;
        using var context = new DataBaseContext();
        Years =
        [
            ..context
                .THistories
                .Where(s => s.Date.HasValue)
                .Select(s => s.Date!.Value.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .Select(y => y.ToString())
        ];

        if (Years.Count.Equals(0))
        {
            Years.Add(DateTime.Now.Year.ToString());
        }

        SelectedYear = now.Year.ToString();
        SelectedMonth = Months[now.Month - 1];

        CurrentVTotalByAccount = context.VTotalByAccounts.FirstOrDefault();

        InitializeComponent();
    }

    private void UpdateMonthLanguage()
    {
        var currentCulture = CultureInfo.CurrentCulture;

        var months = currentCulture.DateTimeFormat.MonthNames
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => s.ToFirstCharUpper()).ToImmutableArray();

        if (Months.Count is 0)
        {
            Months.AddRange(months);
        }
        else
        {
            var selectedMonth = Months.FirstOrDefault(month => month.Equals(SelectedMonth)) ?? string.Empty;
            for (var i = 0; i < months.Length; i++)
            {
                Months[i] = months[i];
            }

            SelectedMonth = selectedMonth;
        }
    }

    //TODO work
    private void CustomPicker_OnSelectedIndexChanged(object? sender, EventArgs e)
    {
        // RefreshDataGrid();
    }
}
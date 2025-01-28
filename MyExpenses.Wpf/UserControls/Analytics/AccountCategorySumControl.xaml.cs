using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Groups.VAccountCategoryMonthlySums;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Converters.Analytics;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountCategorySumControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(AccountCategorySumControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }

    public AccountCategorySumControl(int accountId)
    {
        AccountId = accountId;

        UpdateTextPaint();
        SetChart();
        UpdateLanguage();

        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void Interface_OnThemeChanged()
    {
        UpdateTextPaint();
        UpdateAxisTextPaint();
    }

    #endregion

    #region Function

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();

        var groupsByCategories = context.AnalysisVAccountCategoryMonthlySums
            .Where(s => s.AccountFk == AccountId)
            .GroupBy(s => new { s.AccountFk, s.Account, s.Period, s.CategoryType, s.ColorCode, s.CurrencyFk, s.Currency })
            .Select(g => new GroupsByCategories
            {
                AccountFk = g.Key.AccountFk,
                Account = g.Key.Account,
                Period = g.Key.Period,
                CategoryType = g.Key.CategoryType,
                ColorCode = g.Key.ColorCode,
                SumMonthlySum = Math.Round(g.Sum(v => v.MonthlySum ?? 0), 2),
                CurrencyFk = g.Key.CurrencyFk,
                Currency = g.Key.Currency
            })
            .OrderBy(s => s.Period).ThenBy(s => s.CategoryType)
            .AsEnumerable()
            .GroupBy(s => s.CategoryType)
            .ToList();

        if (groupsByCategories.Count is 0) return;

        var axis = groupsByCategories.First().Select(s => s.Period!);

        SetSeries(groupsByCategories);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<string?, GroupsByCategories>> groupsByCategories)
    {
        var currency = groupsByCategories.First().Select(s => s.Currency).First();

        var series = new List<ISeries>();

        foreach (var groupsByCategory in groupsByCategories)
        {
            var name = groupsByCategory.Key;
            var colorCode = groupsByCategory.First().ColorCode;
            var solidColorPaint = colorCode.ToSolidColorPaint();
            var values = groupsByCategory.Select(s => Math.Round(s.SumMonthlySum ?? 0, 2))
                .ToList();

            var columnSeries = new ColumnSeries<double>
            {
                Name = name,
                Fill = solidColorPaint,
                Values = values,
                YToolTipLabelFormatter = point => $"{point.Model} {currency}"
            };

            series.Add(columnSeries);
        }
        Series = [..series];
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();

        var axis = new Axis
        {
            Labels = transformedLabels,
            LabelsPaint = TextPaint
        };
        XAxis = [axis];
    }

    private void SetYAxis()
    {
        var axis = new Axis
        {
            LabelsPaint = TextPaint
        };
        YAxis = [axis];
    }

    private void UpdateAxisTextPaint()
    {
        for (var i = 0; i < YAxis.Length; i++)
        {
            var tmp = YAxis[i] as Axis;
            tmp!.LabelsPaint = TextPaint;
            YAxis[i] = tmp;
        }

        for (var i = 0; i < XAxis.Length; i++)
        {
            var tmp = XAxis[i] as Axis;
            tmp!.LabelsPaint = TextPaint;
            XAxis[i] = tmp;
        }
    }

    private void UpdateLanguage()
    {
        for (var i = 0; i < XAxis.Length; i++)
        {
            var tmp = XAxis[i] as Axis;
            tmp!.Labels = tmp.Labels!
                .ToTransformLabelsToTitleCaseDateFormatConvertBack()
                .ToTransformLabelsToTitleCaseDateFormat();
            XAxis[i] = tmp;
        }

        UpdateLayout();
    }

    #endregion
}
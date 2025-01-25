using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Groups.VAccountModePaymentCategoryMonthlySums;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsModePaymentMonthlySumControls;

public partial class AccountModePaymentMonthlySumControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(AccountModePaymentMonthlySumControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }

    public AccountModePaymentMonthlySumControl(int accountId)
    {
        AccountId = accountId;

        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();
        UpdateLanguage();

        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    private void Interface_OnLanguageChanged()
        => UpdateLanguage();

    private void Interface_OnThemeChanged()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = new SolidColorPaint(skColor);

        UpdateAxisTextPaint();
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

        for (var i = 0; i < Series.Length; i++)
        {
            var tmp = Series[i] as ColumnSeries<double>;
            tmp!.DataLabelsPaint = new SolidColorPaint(TextPaint.Color);
            Series[i] = tmp;
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

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsByModePaymentCategory = context.AnalysisVAccountModePaymentCategoryMonthlySums
            .Where(s => s.AccountFk == AccountId)
            .GroupBy(v => new { v.AccountFk, v.Account, v.ModePayment, v.Period, v.CurrencyFk, v.Currency })
            .Select(g => new GroupsByModePaymentCategory
            {
                AccountFk = g.Key.AccountFk,
                Account = g.Key.Account,
                ModePayment = g.Key.ModePayment,
                Period = g.Key.Period,
                TotalMonthlySum = g.Sum(v => Math.Round(v.MonthlySum ?? 0, 2)),
                CurrencyFk = g.Key.CurrencyFk,
                Currency = g.Key.Currency,
                TotalMonthlyModePayment = g.Sum(v => v.MonthlyModePayment)
            })
            .OrderBy(s => s.Period).ThenBy(s => s.ModePayment)
            .AsEnumerable()
            .GroupBy(s => s.ModePayment).ToList();

        if (groupsByModePaymentCategory.Count is 0) return;

        var axis = groupsByModePaymentCategory.First().Select(s => s.Period!);

        SetSeries(groupsByModePaymentCategory);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetYAxis()
    {
        var axis = new Axis
        {
            LabelsPaint = TextPaint
        };
        YAxis = [axis];
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

    private void SetSeries(List<IGrouping<string?, GroupsByModePaymentCategory>> groupsByModePayments)
    {
        var currency = groupsByModePayments.First().Select(s => s.Currency).First();
        var series = new List<ISeries>();

        foreach (var groupsByCategory in groupsByModePayments)
        {
            var name = groupsByCategory.Key;

            var monthlyPaymentDataPoints = groupsByCategory
                .GroupBy(s => s.Period)
                .Select(g => new
                {
                    MonthlySum = Math.Round(g.Sum(v => v.TotalMonthlySum ?? 0), 2),
                    MonthlyModePayment = g.Sum(v => v.TotalMonthlyModePayment)
                })
                .ToList();

            var columnSeries = new ColumnSeries<double>
            {
                Name = name,
                Values = monthlyPaymentDataPoints.Select(s => s.MonthlySum).ToList(),
                YToolTipLabelFormatter = point => $"{point.Model} {currency}",
                DataLabelsFormatter = point =>
                {
                    var index = point.Index;
                    var dataPoint = monthlyPaymentDataPoints[index];
                    var count = dataPoint.MonthlyModePayment is 0 ? string.Empty : dataPoint.MonthlyModePayment.ToString()!;
                    return count;
                },
                DataLabelsPaint = new SolidColorPaint(TextPaint.Color),
                DataLabelsPosition = DataLabelsPosition.Middle
            };

            series.Add(columnSeries);
        }
        Series = [..series];
    }
}
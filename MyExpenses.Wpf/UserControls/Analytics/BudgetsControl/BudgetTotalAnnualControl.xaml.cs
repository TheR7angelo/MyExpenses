using System.Windows;
using System.Windows.Data;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.SharedUtils.Maths;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.BudgetsControl;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetTotalAnnualControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(BudgetTotalAnnualControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public BudgetTotalAnnualControl()
    {
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

    private void ViewSourceSeries_OnFilter(object sender, FilterEventArgs e)
        => FilterSeriesTrend.ViewSourceSeries_OnFilter(e);

    private void ViewSourceTrendSeries_OnFilter(object sender, FilterEventArgs e)
        => FilterSeriesTrend.ViewSourceTrendSeries_OnFilter(e);

    #endregion

    #region Function

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

        var configuration = Config.Configuration;
        var primarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodePrimaryColor.ToSolidColorPaint();
        var secondarySolidColorPaint = configuration.Interface.Theme.HexadecimalCodeSecondaryColor.ToSolidColorPaint();

        Span<SolidColorPaint?> solidColorPaints = [secondarySolidColorPaint, primarySolidColorPaint];

        for (var i = 0; i < Series.Length; i++)
        {
            var tmp = Series[i] as ColumnSeries<double>;
            tmp!.Fill = solidColorPaints[i];
            Series[i] = tmp;
        }
    }

    private void UpdateLanguage()
    {
        var trend = BudgetsControlResources.Trend;
        var global = BudgetsControlResources.Global;

        foreach (var iSeries in Series)
        {
            var series = (LineSeries<double>)iSeries;
            if (series.Tag is not IsSeriesTranslatable { IsTranslatable: true } isSeriesTranslatable) continue;

            var newName = isSeriesTranslatable.IsGlobal
                ? isSeriesTranslatable.IsTrend ? $"{global} {trend}" : global
                : $"{isSeriesTranslatable.OriginalName} {trend}";

            series.Name = newName;
        }

        UpdateLayout();
    }

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var records = context.AnalysisVBudgetTotalAnnuals
            .AsEnumerable()
            .GroupBy(s => s.Period)
            .ToList();

        if (records.Count is 0) return;

        var axis = records.Select(s => (int)s.Key!);

        SetSeriesGlobal();
        SetSeries(records);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<int?, AnalysisVBudgetTotalAnnual>> records)
    {
        var trend = BudgetsControlResources.Trend;

        var currency = records.First().Select(s => s.Symbol).First();
        var series = new List<ISeries>();

        var recordsByAccounts = records.SelectMany(s => s)
            .GroupBy(s => s.AccountFk);

        foreach (var recordsByAccount in recordsByAccounts)
        {
            var name = recordsByAccount.First().AccountName;
            var values = recordsByAccount.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();

            var analysisVBudgetMonthlies = recordsByAccount.Select(s => s)
                .ToList();
            var lineSeries = new LineSeries<double>
            {
                Name = name,
                Values = values,
                YToolTipLabelFormatter = point =>
                {
                    var dataPoint = analysisVBudgetMonthlies[point.Index];
                    return $"{dataPoint.PeriodValue} {currency}{Environment.NewLine}" +
                           $"{dataPoint.Status} {dataPoint.DifferenceValue ?? 0:F2}{currency} ({dataPoint.Percentage}%)";
                }
            };

            var trendValues = values.GenerateLinearTrendValues();

            var trendName = $"{name} {trend}";
            var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };
            var trendSeries = new LineSeries<double>
            {
                Name = trendName,
                Values = trendValues.ToList(),
                Fill = null,
                YToolTipLabelFormatter = point => $"{point.Model} {currency}",
                IsVisible = false,
                GeometrySize = 0,
                Tag = isSeriesTranslatableTrend
            };

            series.Add(lineSeries);
            series.Add(trendSeries);
        }

        var newSeries = Series.ToList();
        newSeries.AddRange(series);
        Series = [..newSeries];
    }

    private void SetSeriesGlobal()
    {
        using var context = new DataBaseContext();
        var records = context.AnalysisVBudgetTotalAnnualGlobals.ToList();

        var trend = BudgetsControlResources.Trend;
        var series = new List<ISeries>();

        var name = BudgetsControlResources.Global;
        var values = records.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();

        var lineSeries = new LineSeries<double>
        {
            Name = name,
            Values = values,
            YToolTipLabelFormatter = point =>
            {
                var dataPoint = records[point.Index];
                return $"{dataPoint.PeriodValue}{Environment.NewLine}" +
                       $"{dataPoint.Status} {dataPoint.DifferenceValue ?? 0:F2} ({dataPoint.Percentage}%)";
            },
            Tag = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true, IsGlobal = true, IsTrend = false }
        };

        var trendValues = values.GenerateLinearTrendValues();

        var trendName = $"{name} {trend}";
        var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true, IsGlobal = true };
        var trendSeries = new LineSeries<double>
        {
            Name = trendName,
            Values = trendValues.ToList(),
            Fill = null,
            YToolTipLabelFormatter = point => $"{point.Model}",
            IsVisible = false,
            GeometrySize = 0,
            Tag = isSeriesTranslatableTrend
        };

        series.Add(lineSeries);
        series.Add(trendSeries);

        Series = [..series];
    }

    private void SetXAxis(IEnumerable<int> labels)
    {
        var transformedLabels = labels.Select(s => s.ToString()).ToList();

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

    #endregion
}
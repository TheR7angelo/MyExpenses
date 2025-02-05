using System.Windows;
using System.Windows.Data;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Share.Core.Analysis;
using MyExpenses.SharedUtils.Converters;
using MyExpenses.SharedUtils.Maths;
using MyExpenses.Sql.Context;
using MyExpenses.Sql.Queries;
using MyExpenses.Utils;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.BudgetsControl;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetAnnualControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(BudgetAnnualControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public BudgetAnnualControl()
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

    #endregion

    #region Function

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

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
    }

    private void SetChart()
    {
        var records = EntityQueriesAnalysis.GetVBudgetPeriodAnnuals();

        if (records.Count is 0) return;

        var axis = records.Select(s => s.Key!);

        SetSeriesGlobal();
        SetSeries(records);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<string?, AnalysisVBudgetPeriodAnnual>> records)
    {
        var currency = records.First().Select(s => s.Symbol).First()!;

        var recordsByAccounts = records.SelectMany(s => s)
            .GroupBy(s => s.AccountFk);

        var series = recordsByAccounts.GenerateSeries(BudgetsControlResources.Trend, currency);

        var newSeries = Series.ToList();
        newSeries.AddRange(series);
        Series = [..newSeries];
    }

    private void SetSeriesGlobal()
    {
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        using var context = new DataBaseContext();
        var records = context.AnalysisVBudgetPeriodAnnualGlobals.ToArray();

        var name = BudgetsControlResources.Global;
        var values = records.Select(s => Math.Round(s.PeriodValue ?? 0, 2)).ToList();

        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var isSeriesTranslatable = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true, IsGlobal = true, IsTrend = false };
        var lineSeries = name.CreateLineSeries(values, point =>
        {
            var dataPoint = records[point.Index];
            return $"{dataPoint.PeriodValue}{Environment.NewLine}" +
                   $"{dataPoint.Status} {dataPoint.DifferenceValue ?? 0:F2} ({dataPoint.Percentage}%)";
        }, tag: isSeriesTranslatable);

        var trendValues = values.GenerateLinearTrendValues();

        var trendName = $"{name} {BudgetsControlResources.Trend}";
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true, IsGlobal = true };
        var trendSeries = trendName.CreateLineSeries(trendValues, point => $"{point.Model}", null, false, 0,
            isSeriesTranslatableTrend);

        Series = [lineSeries, trendSeries];
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();
        var axis = transformedLabels.CreateAxis(TextPaint);
        XAxis = [axis];
    }

    private void SetYAxis()
    {
        var axis = TextPaint.CreateAxis();
        YAxis = [axis];
    }

    #endregion

    private void ViewSourceSeries_OnFilter(object sender, FilterEventArgs e)
        => FilterSeriesTrend.ViewSourceSeries_OnFilter(e);

    private void ViewSourceTrendSeries_OnFilter(object sender, FilterEventArgs e)
        => FilterSeriesTrend.ViewSourceTrendSeries_OnFilter(e);
}
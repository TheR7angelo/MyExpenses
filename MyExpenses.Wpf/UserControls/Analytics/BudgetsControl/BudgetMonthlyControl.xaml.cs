using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;
using MyExpenses.Utils;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.BudgetsControl;
using MyExpenses.Wpf.Utils;
using SkiaSharp;

namespace MyExpenses.Wpf.UserControls.Analytics.BudgetsControl;

public partial class BudgetMonthlyControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(BudgetMonthlyControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public List<CheckBox> CheckBoxes { get; } = [];
    public List<CheckBox> CheckBoxesTrend { get; } = [];

    public BudgetMonthlyControl()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();
        UpdateLanguage();

        InitializeComponent();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
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

        var configuration = MyExpenses.Utils.Config.Configuration;
        var primaryColor = ((Color)configuration.Interface.Theme.HexadecimalCodePrimaryColor.ToColor()!).ToSkColor();
        var secondaryColor = ((Color)configuration.Interface.Theme.HexadecimalCodeSecondaryColor.ToColor()!).ToSkColor();
        var skColors = new List<SKColor> { secondaryColor, primaryColor };

        for (var i = 0; i < Series.Length; i++)
        {
            var tmp = Series[i] as ColumnSeries<double>;
            tmp!.Fill = new SolidColorPaint(skColors[i]);
            Series[i] = tmp;
        }
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

            var name = series.Name!;
            var checkBox = CheckBoxesTrend.FirstOrDefault(s => s.Content.Equals(name)) ?? CheckBoxes.First(s => s.Content.Equals(name));

            var newName = isSeriesTranslatable.IsGlobal
                ? isSeriesTranslatable.IsTrend ? $"{global} {trend}" : global
                : $"{isSeriesTranslatable.OriginalName} {trend}";

            series.Name = newName;
            checkBox.Content = newName;
        }

        UpdateLayout();
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var records = context.AnalysisVBudgetMonthlies
            .AsEnumerable()
            .GroupBy(s => s.Period)
            .ToList();

        if (records.Count is 0) return;

        var axis = records.Select(s => s.Key!);

        SetSeriesGlobal();
        SetSeries(records);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<string?, AnalysisVBudgetMonthly>> records)
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

            var xData = Enumerable.Range(1, values.Count).Select(i => (double)i).ToArray();
            var (a, b) = AnalyticsUtils.CalculateLinearTrend(xData, values.ToArray());
            var trendValues = xData.Select(x => Math.Round(a * x + b, 2)).ToArray();

            var trendName = $"{name} {trend}";
            var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };
            var trendSeries = new LineSeries<double>
            {
                Name = trendName,
                Values = trendValues,
                Fill = null,
                YToolTipLabelFormatter = point => $"{point.Model} {currency}",
                IsVisible = false,
                GeometrySize = 0,
                Tag = isSeriesTranslatableTrend
            };

            series.Add(lineSeries);
            series.Add(trendSeries);

            var checkBox = new CheckBox
            {
                Content = name,
                IsChecked = lineSeries.IsVisible,
                Margin = new Thickness(5)
            };
            checkBox.Click += (_, _) =>
            {
                {
                    lineSeries.IsVisible = !lineSeries.IsVisible;
                }
            };
            CheckBoxes.Add(checkBox);

            var checkBoxTrend = new CheckBox
            {
                Content = trendName,
                IsChecked = trendSeries.IsVisible,
                Margin = new Thickness(5)
            };
            checkBoxTrend.Click += (_, _) =>
            {
                {
                    trendSeries.IsVisible = !trendSeries.IsVisible;
                }
            };
            CheckBoxesTrend.Add(checkBoxTrend);
        }

        var newSeries = Series.ToList();
        newSeries.AddRange(series);
        Series = [..newSeries];
    }

    private void SetSeriesGlobal()
    {
        using var context = new DataBaseContext();
        var records = context.AnalysisVBudgetMonthlyGlobals.ToList();

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

        var xData = Enumerable.Range(1, values.Count).Select(i => (double)i).ToArray();
        var (a, b) = AnalyticsUtils.CalculateLinearTrend(xData, values.ToArray());
        var trendValues = xData.Select(x => Math.Round(a * x + b, 2)).ToArray();

        var trendName = $"{name} {trend}";
        var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true, IsGlobal = true };
        var trendSeries = new LineSeries<double>
        {
            Name = trendName,
            Values = trendValues,
            Fill = null,
            YToolTipLabelFormatter = point => $"{point.Model}",
            IsVisible = false,
            GeometrySize = 0,
            Tag = isSeriesTranslatableTrend
        };

        series.Add(lineSeries);
        series.Add(trendSeries);

        var checkBox = new CheckBox
        {
            Content = name,
            IsChecked = lineSeries.IsVisible,
            Margin = new Thickness(5)
        };
        checkBox.Click += (_, _) =>
        {
            {
                lineSeries.IsVisible = !lineSeries.IsVisible;
            }
        };
        CheckBoxes.Add(checkBox);

        var checkBoxTrend = new CheckBox
        {
            Content = trendName,
            IsChecked = trendSeries.IsVisible,
            Margin = new Thickness(5)
        };
        checkBoxTrend.Click += (_, _) =>
        {
            {
                trendSeries.IsVisible = !trendSeries.IsVisible;
            }
        };
        CheckBoxesTrend.Add(checkBoxTrend);

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

    #endregion
}
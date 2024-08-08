using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Models.Wpf.Charts;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.AccountValueTrendControl;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountValueTrendControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(AccountValueTrendControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private List<CheckBox> CheckBoxes { get; } = [];
    private List<CheckBox> CheckBoxesTrend { get; } = [];

    public AccountValueTrendControl()
    {
        var skColor = GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();
        UpdateLanguage();

        InitializeComponent();

        SetButtonPanel();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        Interface.LanguageChanged += Interface_OnLanguageChanged;
    }

    #region Action

    private void Interface_OnLanguageChanged(object sender, ConfigurationLanguageChangedEventArgs e)
        => UpdateLanguage();

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
    {
        var skColor = GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        UpdateAxisTextPaint();
    }

    #endregion

    #region Function

    private static (double a, double b) CalculateLinearTrend(double[] xData, double[] yData)
    {
        double sumX = 0, sumY = 0, sumXy = 0, sumXx = 0;

        var n = xData.Length;
        for (var i = 0; i < n; i++)
        {
            sumX += xData[i];
            sumY += yData[i];
            sumXy += xData[i] * yData[i];
            sumXx += xData[i] * xData[i];
        }

        var a = (n * sumXy - sumX * sumY) / (n * sumXx - sumX * sumX); // slope
        var b = (sumY / n) - (a * sumX / n); // intercept

        return (a, b);
    }

    private SKColor GetSkColor()
    {
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }

    private void SetButtonPanel()
    {
        CheckBoxes.ForEach(s => CheckboxPanel.Children.Add(s));
        CheckBoxesTrend.ForEach(s => CheckboxTrendPanel.Children.Add(s));
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsVAccountMonthlyCumulativeSums = context.VAccountMonthlyCumulativeSums
            .ToList()
            .GroupBy(s => s.Account)
            .ToList();

        if (groupsVAccountMonthlyCumulativeSums.Count is 0) return;

        var axis = groupsVAccountMonthlyCumulativeSums.First().Select(s => s.Period!);

        SetSeries(groupsVAccountMonthlyCumulativeSums);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<string?, VAccountMonthlyCumulativeSum>> groupsVAccountMonthlyCumulativeSums)
    {
        var trend = AccountValueTrendControlResources.Trend;

        var series = new List<ISeries>();
        foreach (var groupVAccountMonthlyCumulativeSums in groupsVAccountMonthlyCumulativeSums)
        {
            var name = groupVAccountMonthlyCumulativeSums.Key;
            var values = groupVAccountMonthlyCumulativeSums
                .Select(s => (double)s.CumulativeSum!)
                .ToList();

            var lineSeries = new LineSeries<double>
            {
                Name = name,
                Values = values,
                Fill = null
            };

            var xData = Enumerable.Range(1, values.Count).Select(i => (double)i).ToArray();
            var (a, b) = CalculateLinearTrend(xData, values.ToArray());
            var trendValues = xData.Select(x => Math.Round(a * x + b, 2)).ToArray();

            var trendName = $"{name} {trend}";
            var isSeriesTranslatableTrend = new IsSeriesTranslatable { OriginalName = name, IsTranslatable = true };
            var trendSeries = new LineSeries<double>
            {
                Name = trendName,
                Values = trendValues,
                Fill = null,
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

        Series = series.ToArray();
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

        var trend = AccountValueTrendControlResources.Trend;

        foreach (var iSeries in Series)
        {
            var series = (LineSeries<double>)iSeries;
            if (series.Tag is not IsSeriesTranslatable { IsTranslatable: true } isSeriesTranslatable) continue;

            var name = series.Name!;
            var checkBox = CheckBoxesTrend.First(s => s.Content.Equals(name));

            var newName = $"{isSeriesTranslatable.OriginalName} {trend}";
            series.Name = newName;
            checkBox.Content = newName;
        }

        UpdateLayout();
    }

    #endregion
}
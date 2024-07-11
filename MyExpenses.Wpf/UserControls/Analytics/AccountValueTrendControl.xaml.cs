using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountValueTrendControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(AccountValueTrendControl), new PropertyMetadata(default(SolidColorPaint)));

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private List<CheckBox> CheckBoxes { get; } = [];
    private List<CheckBox> CheckBoxesTrend { get; } = [];

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public AccountValueTrendControl()
    {
        var skColor = GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();

        Configuration.ConfigurationChanged += Configuration_OnConfigurationChanged;
        InitializeComponent();

        SetButtonPanel();
    }

    private void Configuration_OnConfigurationChanged(object sender, ConfigurationChangedEventArgs e)
    {
        var skColor = GetSkColor();
        TextPaint = new SolidColorPaint(skColor);
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

        var axis = groupsVAccountMonthlyCumulativeSums.First().Select(s => s.Period!);

        SetSeries(groupsVAccountMonthlyCumulativeSums);
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
        var transformedLabels = labels.Select(label =>
        {
            var labelSplit = label.Split('-');

            if (!int.TryParse(labelSplit[0], out var year) || !int.TryParse(labelSplit[1], out var month))
                throw new FormatException($"Invalid format for label '{label}'. Expected 'YYYY-MM' format.");

            var d = new DateOnly(year, month, 1);
            var newLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.ToString("Y"));

            return newLabel;
        }).ToList();

        var axis = new Axis
        {
            Labels = transformedLabels,
            LabelsPaint = TextPaint
        };
        XAxis = [axis];
    }

    private void SetSeries(List<IGrouping<string?, VAccountMonthlyCumulativeSum>> groupsVAccountMonthlyCumulativeSums)
    {
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
            var trendSeries = new LineSeries<double>
            {
                Name = $"{name} Trend",
                Values = trendValues,
                Fill = null,
                IsVisible = false,
                GeometrySize = 0
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
                Content = $"{name} trend",
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
}
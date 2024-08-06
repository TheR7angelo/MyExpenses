using System.Windows;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class CumulativeTotalSumChartControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(CumulativeTotalSumChartControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; private set; } = null!;

    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public CumulativeTotalSumChartControl()
    {
        var skColor = GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();

        Interface.ThemeChanged += Interface_OnThemeChanged;
        InitializeComponent();
    }

    private void Interface_OnThemeChanged(object sender, ConfigurationThemeChangedEventArgs e)
    {
        var skColor = GetSkColor();
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
    }

    private SKColor GetSkColor()
    {
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsByPeriods = context.VAccountMonthlyCumulativeSums
            .OrderBy(s => s.Period).ThenBy(s => s.AccountFk)
            .ToList()
            .GroupBy(s => s.Period)
            .ToList();

        if (groupsByPeriods.Count is 0) return;

        var axis = groupsByPeriods.Select(s => s.Key!);

        SetSeries(groupsByPeriods);

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

    private void SetSeries(List<IGrouping<string?, VAccountMonthlyCumulativeSum>> groupsByPeriods)
    {
        var sums = new List<double>();
        foreach (var groupsByPeriod in groupsByPeriods)
        {
            var value = groupsByPeriod.Select(s => Math.Round(s.CumulativeSum ?? 0, 2)).Sum();
            sums.Add(value);
        }

        var columnSeries = new ColumnSeries<double>
        {
            Values = sums,
            Name = "Totals"
        };
        Series = [columnSeries];
    }
}
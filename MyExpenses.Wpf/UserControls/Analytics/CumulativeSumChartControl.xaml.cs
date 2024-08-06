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

public partial class CumulativeSumChartControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(CumulativeSumChartControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; private set; } = null!;

    public ICartesianAxis[] XAxis { get; set; } = null!;

    public CumulativeSumChartControl()
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
            .GroupBy(s => s.AccountFk)
            .ToList();

        if (groupsByPeriods.Count is 0) return;

        var axis = groupsByPeriods.First().Select(s => s.Period!);

        SetXAxis(axis);
        SetSeries(groupsByPeriods);
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

    private void SetSeries(List<IGrouping<int?, VAccountMonthlyCumulativeSum>> groupsByAccounts)
    {
        var series = new List<ISeries>();

        foreach (var groupsByAccount in groupsByAccounts)
        {
            var values = groupsByAccount.Select(s => Math.Round(s.CumulativeSum ?? 0, 2));

            var stakedColumnSeries = new StackedColumnSeries<double>
            {
                Values = values,
                Name = groupsByAccount.First().Account,
                Stroke = null,
            };
            series.Add(stakedColumnSeries);
        }

        Series = series.ToArray();
    }
}
using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Resources.Resx.UserControls.Analytics.CumulativeTotalSumChartControl;

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

    #endregion

    #region Function

    private List<double> CalculatePreviousDeltas(List<double> sums)
    {
        var deltas = new List<double> { 0d };

        for (var i = 1; i < sums.Count; i++)
        {
            double calc;
            if (sums[i - 1] < 0)
            {
                calc = sums[i] + Math.Abs(sums[i - 1]);
            }
            else
            {
                calc = sums[i] - sums[i - 1];
            }

            deltas.Add(calc);
        }

        return deltas;
    }

    private void SetChart()
    {
        // TODO optimise
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
            Name = CumulativeTotalSumChartControlResources.ColumnSeriesTotalName
        };

        var previousDeltas = CalculatePreviousDeltas(sums);
        var deltaSeries = new LineSeries<double>
        {
            Values = previousDeltas,
            Name = CumulativeTotalSumChartControlResources.LineSeriesPreviousDeltaName,
            Fill = null,
            DataLabelsFormatter = values => values.Coordinate.SecondaryValue.ToString("F2")
        };

        Series = [columnSeries, deltaSeries];
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

        foreach (var series in Series)
        {
            switch (series)
            {
                case LineSeries<double> lineSeries:
                    lineSeries.Name = CumulativeTotalSumChartControlResources.LineSeriesPreviousDeltaName;
                    break;
                case ColumnSeries<double> columnSeries:
                    columnSeries.Name = CumulativeTotalSumChartControlResources.ColumnSeriesTotalName;
                    break;
            }
        }

        UpdateLayout();
    }

    #endregion
}
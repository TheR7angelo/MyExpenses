using System.Windows;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Config.Interfaces;
using MyExpenses.Models.Sql.Bases.Views.Analysis;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Utils;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class CumulativeSumChartControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        // ReSharper disable once HeapView.ObjectAllocation.Evident
        typeof(SolidColorPaint), typeof(CumulativeSumChartControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; private set; } = null!;

    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public CumulativeSumChartControl()
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

    private void UpdateTextPaint()
    {
        var skColor = Utils.Resources.GetMaterialDesignBodySkColor();
        TextPaint = skColor.ToSolidColorPaint();
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsByPeriods = context.AnalysisVAccountMonthlyCumulativeSums
            .OrderBy(s => s.Period).ThenBy(s => s.AccountFk)
            .AsEnumerable()
            .GroupBy(s => s.AccountFk)
            .ToList();

        if (groupsByPeriods.Count is 0) return;

        var axis = groupsByPeriods.First().Select(s => s.Period!);

        SetSeries(groupsByPeriods);

        SetXAxis(axis);
        SetYAxis();
    }

    private void SetSeries(List<IGrouping<int?, AnalysisVAccountMonthlyCumulativeSum>> groupsByAccounts)
    {
        var currency = groupsByAccounts.First().Select(s => s.Currency).First();

        var series = new List<ISeries>();

        foreach (var groupsByAccount in groupsByAccounts)
        {
            var values = groupsByAccount.Select(s => Math.Round(s.CumulativeSum ?? 0, 2))
                .ToList();

            var stakedColumnSeries = new StackedColumnSeries<double>
            {
                Values = values,
                Name = groupsByAccount.First().Account,
                YToolTipLabelFormatter = point => $"{point.Model} {currency}"
            };

            series.Add(stakedColumnSeries);
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
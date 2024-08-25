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
using MyExpenses.Wpf.Utils;
using SkiaSharp;

namespace MyExpenses.Wpf.UserControls.Analytics.AccountsCategorySumPositiveNegativeControls;

public partial class AccountCategorySumPositiveNegativeControl
{
    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(AccountCategorySumPositiveNegativeControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; set; } = null!;
    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    private int AccountId { get; }

    public AccountCategorySumPositiveNegativeControl(int accountId)
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
        var records = context.VAccountCategoryMonthlySumPositiveNegatives
            .Where(s => s.AccountFk == AccountId)
            .AsEnumerable()
            .GroupBy(s => s.Period)
            .ToList();
        
        if (records.Count is 0) return;

        var axis = records.Select(s => s.Key!);

        SetSeries(records);
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

    private void SetSeries(List<IGrouping<string?, VAccountCategoryMonthlySumPositiveNegative>> records)
    {
        var negativeSeries = new List<ISeries>();
        var positiveSeries = new List<ISeries>();

        var newRecords = records.SelectMany(s => s)
            .GroupBy(s => s.CategoryType)
            .ToList();
    
        foreach (var record in newRecords)
        {
            var name = record.Key;
        
            var colorCode = record.First().ColorCode;
            var skColor = (SKColor)colorCode!.ToSkColor()!;
        
            var negativeValues = record.Select(s => Math.Round(s.MonthlyNegativeSum ?? 0));
            var positiveValues = record.Select(s => Math.Round(s.MonthlyPositiveSum ?? 0));

            var negativeStackedColumnSeries = new StackedColumnSeries<double>
            {
                Name = name,
                Values = negativeValues,
                Fill = new SolidColorPaint(skColor),
                StackGroup = 0
            };
        
            var positiveStackedColumnSeries = new StackedColumnSeries<double>
            {
                Name = name,
                IsVisibleAtLegend = false,
                Values = positiveValues,
                Fill = new SolidColorPaint(skColor),
                StackGroup = 1
            };

            negativeSeries.Add(negativeStackedColumnSeries);
            positiveSeries.Add(positiveStackedColumnSeries);
        }

        var series = negativeSeries.Concat(positiveSeries);
        Series = [..series];
    }
}
using System.Windows;
using System.Windows.Media;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using MyExpenses.Models.Sql.Tables;
using MyExpenses.Models.Sql.Views;
using MyExpenses.Sql.Context;
using MyExpenses.Wpf.Converters.Analytics;
using MyExpenses.Wpf.Utils;
using SkiaSharp;
using SkiaSharp.Views.WPF;

namespace MyExpenses.Wpf.UserControls.Analytics;

public partial class AccountCategorySumControl
{
    public static readonly DependencyProperty TAccountProperty = DependencyProperty.Register(nameof(TAccount),
        typeof(TAccount), typeof(AccountCategorySumControl), new PropertyMetadata(default(TAccount)));

    public TAccount TAccount
    {
        get => (TAccount)GetValue(TAccountProperty);
        set => SetValue(TAccountProperty, value);
    }

    public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint),
        typeof(SolidColorPaint), typeof(AccountCategorySumControl), new PropertyMetadata(default(SolidColorPaint)));

    public SolidColorPaint TextPaint
    {
        get => (SolidColorPaint)GetValue(TextPaintProperty);
        set => SetValue(TextPaintProperty, value);
    }

    public ISeries[] Series { get; private set; } = null!;

    public ICartesianAxis[] XAxis { get; set; } = null!;
    public ICartesianAxis[] YAxis { get; set; } = null!;

    public AccountCategorySumControl()
    {
        var skColor = GetSkColor();
        TextPaint = new SolidColorPaint(skColor);

        SetChart();

        InitializeComponent();
    }

    private void SetChart()
    {
        using var context = new DataBaseContext();
        var groupsByCategories = context.VAccountCategoryMonthlyCumulativeSums
            .Where(s => s.AccountFk == 1)
            .OrderBy(s => s.Period).ThenBy(s => s.CategoryType)
            .ToList().GroupBy(s => s.CategoryType).ToList();

        if (groupsByCategories.Count is 0) return;

        var axis = groupsByCategories.First().Select(s => s.Period!);

        SetSeries(groupsByCategories);
        SetXAxis(axis);
        SetYAxis();
    }

    private void SetXAxis(IEnumerable<string> labels)
    {
        var transformedLabels = labels.ToTransformLabelsToTitleCaseDateFormat();

        var axis = new Axis
        {
            Labels = transformedLabels,
            SeparatorsPaint = TextPaint,
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

    private void SetSeries(List<IGrouping<string?, VAccountCategoryMonthlySum>> groupsByCategories)
    {
        var series = new List<ISeries<double>>();

        foreach (var groupsByCategory in groupsByCategories)
        {
            var name = groupsByCategory.Key;
            var colorCode = groupsByCategory.First().ColorCode;
            var skColor = (SKColor)colorCode!.ToSkColor()!;
            var values = groupsByCategory.Select(s => Math.Round(s.MonthlySum ?? 0, 2));

            var columnSeries = new ColumnSeries<double>
            {
                Name = name,
                Fill = new SolidColorPaint(skColor),
                Values = values
            };

            series.Add(columnSeries);
        }

        Series = [..series];
    }

    private SKColor GetSkColor()
    {
        var brush = (SolidColorBrush)FindResource("MaterialDesignBody");
        var wpfColor = brush.Color;
        var skColor = wpfColor.ToSKColor();
        return skColor;
    }
}